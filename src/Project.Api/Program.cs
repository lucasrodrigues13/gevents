using System.Net;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Project.Api.Conta;
using Project.Infrastructure;
using Project.Infrastructure.Conta;

var builder = WebApplication.CreateBuilder(args);

var segredoJwt = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(segredoJwt) || segredoJwt.Length < 32)
{
    throw new InvalidOperationException("O segredo do JWT não está configurado.");
}

var chaves = builder.Configuration["DataProtection:KeysPath"];
if (string.IsNullOrWhiteSpace(chaves))
{
    chaves = "keys";
}

Directory.CreateDirectory(chaves);
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(chaves));
builder.Services.AddInfraestrutura(builder.Configuration);

var limites = builder.Configuration.GetSection("RateLimit");
var cadastroPorHora = limites.GetValue("CadastroPorHora", 5);
var loginPorMinuto = limites.GetValue("LoginPorMinuto", 10);
var recuperacaoPorHora = limites.GetValue("RecuperacaoPorHora", 3);
var reenvioPorHora = limites.GetValue("ReenvioPorHora", 3);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (contexto, ct) =>
    {
        if (contexto.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            contexto.HttpContext.Response.Headers.RetryAfter =
                Math.Ceiling(retryAfter.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        contexto.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await contexto.HttpContext.Response.WriteAsJsonAsync(
            new ErroResposta("limite", "A ação não pode ser repetida agora."),
            ct);
    };

    options.AddPolicy("cadastro", http => Janela(http, cadastroPorHora, TimeSpan.FromHours(1)));
    options.AddPolicy("login", http => Janela(http, loginPorMinuto, TimeSpan.FromMinutes(1)));
    options.AddPolicy("recuperacao", http => Janela(http, recuperacaoPorHora, TimeSpan.FromHours(1)));
    options.AddPolicy("reenvio", http => Janela(http, reenvioPorHora, TimeSpan.FromHours(1)));
});

var origens = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://localhost:4200"];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.WithOrigins(origens).AllowAnyHeader().AllowAnyMethod());
});

var issuer = builder.Configuration["Jwt:Issuer"] ?? "gevents";
var audience = builder.Configuration["Jwt:Audience"] ?? "gevents";
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(segredoJwt)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
        };
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnChallenge = async contexto =>
            {
                contexto.HandleResponse();
                contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await contexto.Response.WriteAsJsonAsync(
                    new ErroResposta("autenticacao", "É preciso entrar para continuar."));
            },
            OnForbidden = async contexto =>
            {
                contexto.Response.StatusCode = StatusCodes.Status403Forbidden;
                await contexto.Response.WriteAsJsonAsync(
                    new ErroResposta("autorizacao", "Você não tem acesso a esta área."));
            },
        };
    });
builder.Services.AddAuthorization();

var swagger = builder.Configuration.GetValue("Swagger:Enabled", false);
if (swagger)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
    foreach (var rede in builder.Configuration.GetSection("ForwardedHeaders:KnownNetworks").Get<string[]>() ?? [])
    {
        var partes = rede.Split('/');
        if (partes.Length == 2 && IPAddress.TryParse(partes[0], out var endereco) && int.TryParse(partes[1], out var prefixo))
        {
            options.KnownIPNetworks.Add(new System.Net.IPNetwork(endereco, prefixo));
        }
    }
});

var app = builder.Build();

app.UseExceptionHandler(handler =>
{
    handler.Run(async contexto =>
    {
        var falha = contexto.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var logger = contexto.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("gevents");
        logger.LogError(
            "Falha interna {Tipo} ao processar {Metodo} {Caminho}.",
            falha?.Error.GetType().Name,
            contexto.Request.Method,
            contexto.Request.Path);
        contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await contexto.Response.WriteAsJsonAsync(new ErroResposta("erro", "Não foi possível concluir a ação."));
    });
});
app.UseForwardedHeaders();
if (swagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));
ContaEndpoints.Mapear(app);

await using (var escopo = app.Services.CreateAsyncScope())
{
    await escopo.ServiceProvider.GetRequiredService<GarantiaDePapeis>().GarantirAsync();
    await escopo.ServiceProvider.GetRequiredService<BootstrapAdministrador>().ExecutarAsync();
}

app.Run();

static RateLimitPartition<string> Janela(HttpContext http, int limite, TimeSpan janela)
{
    var ip = http.Connection.RemoteIpAddress?.ToString();
    var chave = string.IsNullOrWhiteSpace(ip) ? "desconhecido" : ip;
    return RateLimitPartition.GetFixedWindowLimiter(
        chave,
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = limite,
            Window = janela,
            QueueLimit = 0,
        });
}

public partial class Program;
