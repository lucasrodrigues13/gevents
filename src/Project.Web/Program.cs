using System.Net;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Project.Infrastructure;
using Project.Infrastructure.Identity;
using ColorlibHQ.AdminLTE.AspNetCore;
using Project.Web.Security;

namespace Project.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddSingleton<AccountRateLimiter>();
        builder.Services.AddRazorPages();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAdminLte(builder.Configuration);

        if (builder.Environment.IsProduction() && string.IsNullOrWhiteSpace(builder.Configuration["Storage:Root"]))
        {
            throw new InvalidOperationException("Storage:Root é obrigatório em produção.");
        }

        var maxRequest = builder.Configuration.GetValue("Storage:MaxRequestBytes", 50L * 1024 * 1024);
        builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = maxRequest);
        builder.Services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = maxRequest);

        var keysPath = builder.Configuration["DataProtection:KeysPath"];
        if (string.IsNullOrWhiteSpace(keysPath))
        {
            if (builder.Environment.IsProduction())
            {
                throw new InvalidOperationException("DataProtection:KeysPath é obrigatório em produção.");
            }

            keysPath = Path.Combine(builder.Environment.ContentRootPath, "data-protection-keys");
        }

        Directory.CreateDirectory(keysPath);
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
            .SetApplicationName("gevents");

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();

            foreach (var proxy in builder.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? [])
            {
                if (IPAddress.TryParse(proxy, out var address))
                {
                    options.KnownProxies.Add(address);
                }
            }
        });

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = builder.Environment.IsProduction()
                ? CookieSecurePolicy.Always
                : CookieSecurePolicy.SameAsRequest;
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            if (!app.Environment.IsEnvironment("Testing"))
            {
                app.UseHsts();
            }
        }

        app.UseForwardedHeaders();

        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapRazorPages().WithStaticAssets();
        app.Run();
    }
}
