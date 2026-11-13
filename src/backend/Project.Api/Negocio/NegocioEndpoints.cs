using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Project.Api.Conta;
using Project.Application.Arquivos;
using Project.Application.Cep;
using Project.Application.Comum;
using Project.Application.Negocio;
using Project.Domain.Convidados;
using Project.Domain.Eventos;
using Project.Domain.Tarefas;

namespace Project.Api.Negocio;

public static class NegocioEndpoints
{
    public static void Mapear(WebApplication app)
    {
        var api = app.MapGroup("/api/v1").RequireAuthorization();
        api.MapGet("/perfil", Perfil);
        api.MapPut("/perfil", GravarPerfil);
        api.MapGet("/painel", Painel);
        api.MapGet("/modelos", Modelos);
        api.MapPost("/modelos", CriarModelo);
        api.MapGet("/eventos", Eventos);
        api.MapGet("/eventos/{id:guid}", Evento);
        api.MapPost("/eventos", CriarEvento);
        api.MapPut("/eventos/{id:guid}", AtualizarEvento);
        api.MapPost("/eventos/{id:guid}/concluir", Concluir);
        api.MapPost("/eventos/{id:guid}/cancelar", Cancelar);
        api.MapPost("/eventos/{id:guid}/assessoria", Assessoria);
        api.MapPut("/eventos/{id:guid}/transfer", Transfer);
        api.MapPost("/eventos/{id:guid}/imagem", ImagemEvento).DisableAntiforgery();
        api.MapGet("/eventos/{id:guid}/tarefas", TarefasDoEvento);
        api.MapPost("/eventos/{id:guid}/tarefas", CriarTarefa);
        api.MapGet("/tarefas", Tarefas);
        api.MapPut("/tarefas/{id:guid}", AtualizarTarefa);
        api.MapGet("/clientes", Clientes);
        api.MapPost("/clientes", CriarCliente);
        api.MapPut("/clientes/{id:guid}", AtualizarCliente);
        api.MapPost("/clientes/{id:guid}/foto", FotoCliente).DisableAntiforgery();
        api.MapGet("/eventos/{id:guid}/clientes", ClientesDoEvento);
        api.MapPost("/eventos/{id:guid}/clientes", Vincular);
        api.MapDelete("/eventos/{id:guid}/clientes/{clienteId:guid}", Desvincular);
        api.MapGet("/grupos", Grupos);
        api.MapPost("/grupos", CriarGrupo);
        api.MapGet("/eventos/{id:guid}/convites", Convites);
        api.MapPost("/eventos/{id:guid}/convites", CriarConvite);
        api.MapPut("/convites/{id:guid}", AtualizarConvite);
        api.MapPost("/convites/{id:guid}/convidados", CriarConvidado);
        api.MapPut("/convites/{id:guid}/convidados/{convidadoId:guid}", AtualizarConvidado);
        api.MapDelete("/convidados/{id:guid}", RemoverConvidado);
        api.MapPost("/convites/{id:guid}/principal", Principal);
        api.MapGet("/eventos/{id:guid}/convites/planilha", Planilha);
        api.MapPost("/eventos/{id:guid}/convites/importar", Importar).DisableAntiforgery();
        api.MapGet("/convites/{id:guid}/qr", Qr);
        api.MapGet("/convites/{id:guid}/whatsapp", WhatsApp);
        api.MapGet("/arquivos/{id:guid}", Arquivo);
        api.MapGet("/cep/{cep}", Cep);

        app.MapGet("/api/v1/rsvp/{token}", AbrirRsvp);
        app.MapPost("/api/v1/rsvp/{token}", ResponderRsvp);
    }

    private static Task<IResult> Perfil(ClaimsPrincipal usuario, PerfilServico servico, CancellationToken ct) =>
        ComConta(usuario, async conta => Results.Ok(await servico.ObterAsync(conta, Email(usuario), ct)));

    private static Task<IResult> GravarPerfil(SalvarPerfil pedido, ClaimsPrincipal usuario, PerfilServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.SalvarAsync(conta, Email(usuario), pedido, Hoje(), ct)));

    private static Task<IResult> Painel(ClaimsPrincipal usuario, PainelServico servico, CancellationToken ct) =>
        ComConta(usuario, async conta => Results.Ok(await servico.ObterAsync(conta, ct)));

    private static Task<IResult> Modelos(ClaimsPrincipal usuario, ModeloServico servico, CancellationToken ct) =>
        ComConta(usuario, async conta => Results.Ok(await servico.ListarAsync(conta, ct)));

    private static Task<IResult> CriarModelo(NovoModelo pedido, ClaimsPrincipal usuario, ModeloServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.CriarAsync(conta, pedido, ct)));

    private static Task<IResult> Eventos(
        ClaimsPrincipal usuario,
        EventoServico servico,
        string? situacao,
        string? nome,
        string? tipo,
        DateOnly? de,
        DateOnly? ate,
        string? tag,
        CancellationToken ct) =>
        ComConta(usuario, async conta => Results.Ok(await servico.ListarAsync(
            conta,
            new FiltroEventos(EnumOuNulo<SituacaoEvento>(situacao), nome, EnumOuNulo<TipoEvento>(tipo), de, ate, tag),
            Hoje(),
            ct)));

    private static Task<IResult> Evento(Guid id, ClaimsPrincipal usuario, EventoServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.ObterAsync(conta, id, Hoje(), ct)));

    private static Task<IResult> CriarEvento(SalvarEvento pedido, ClaimsPrincipal usuario, EventoServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.CriarAsync(conta, pedido, Hoje(), ct)));

    private static Task<IResult> AtualizarEvento(Guid id, SalvarEvento pedido, ClaimsPrincipal usuario, EventoServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.AtualizarAsync(conta, id, pedido, Hoje(), ct)));

    private static Task<IResult> Concluir(Guid id, ClaimsPrincipal usuario, EventoServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.EncerrarAsync(conta, id, SituacaoEvento.Concluido, Hoje(), ct)));

    private static Task<IResult> Cancelar(Guid id, ClaimsPrincipal usuario, EventoServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.EncerrarAsync(conta, id, SituacaoEvento.Cancelado, Hoje(), ct)));

    private static Task<IResult> Assessoria(Guid id, AssessoriaPedido pedido, ClaimsPrincipal usuario, EventoServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.DefinirAssessoriaAsync(conta, id, pedido, Hoje(), ct)));

    private static Task<IResult> Transfer(Guid id, TransferPedido pedido, ClaimsPrincipal usuario, EventoServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.DefinirTransferAsync(conta, id, pedido.Ativo, pedido.Locais, Hoje(), ct)));

    private static Task<IResult> ImagemEvento(
        Guid id,
        IFormFile? arquivo,
        ClaimsPrincipal usuario,
        EventoServico servico,
        IArmazenamentoArquivo armazenamento,
        CancellationToken ct) =>
        ComConta(usuario, conta => Imagem(conta, arquivo, armazenamento, async (imagem, token) =>
        {
            var anterior = await servico.GuardarImagemAsync(conta, id, imagem, token);
            return (anterior.Ok, anterior.Falha, anterior.Valor);
        }, ct));

    private static Task<IResult> TarefasDoEvento(Guid id, ClaimsPrincipal usuario, TarefaServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.DoEventoAsync(conta, id, ct)));

    private static Task<IResult> CriarTarefa(Guid id, SalvarTarefa pedido, ClaimsPrincipal usuario, TarefaServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.CriarAsync(conta, id, pedido, ct)));

    private static Task<IResult> Tarefas(ClaimsPrincipal usuario, TarefaServico servico, CancellationToken ct) =>
        ComConta(usuario, async conta => Results.Ok(await servico.DaContaAsync(conta, ct)));

    private static Task<IResult> AtualizarTarefa(Guid id, SalvarTarefa pedido, ClaimsPrincipal usuario, TarefaServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.AtualizarAsync(conta, id, pedido, ct)));

    private static Task<IResult> Clientes(ClaimsPrincipal usuario, ClienteServico servico, string? nome, CancellationToken ct) =>
        ComConta(usuario, async conta => Results.Ok(await servico.ListarAsync(conta, nome, ct)));

    private static Task<IResult> CriarCliente(NovoCliente pedido, ClaimsPrincipal usuario, ClienteServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.CriarAsync(conta, pedido, Hoje(), ct)));

    private static Task<IResult> AtualizarCliente(Guid id, NovoCliente pedido, ClaimsPrincipal usuario, ClienteServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.AtualizarAsync(conta, id, pedido, Hoje(), ct)));

    private static Task<IResult> FotoCliente(
        Guid id,
        IFormFile? arquivo,
        ClaimsPrincipal usuario,
        ClienteServico servico,
        IArmazenamentoArquivo armazenamento,
        CancellationToken ct) =>
        ComConta(usuario, conta => Imagem(conta, arquivo, armazenamento, async (imagem, token) =>
        {
            var anterior = await servico.GuardarFotoAsync(conta, id, imagem, token);
            return (anterior.Ok, anterior.Falha, anterior.Valor);
        }, ct));

    private static Task<IResult> ClientesDoEvento(Guid id, ClaimsPrincipal usuario, ClienteServico servico, CancellationToken ct) =>
        ComConta(usuario, async conta => Results.Ok(await servico.DoEventoAsync(conta, id, ct)));

    private static Task<IResult> Vincular(Guid id, VinculoPedido pedido, ClaimsPrincipal usuario, ClienteServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.VincularAsync(conta, id, pedido.ClienteId, ct)));

    private static Task<IResult> Desvincular(Guid id, Guid clienteId, ClaimsPrincipal usuario, ClienteServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.DesvincularAsync(conta, id, clienteId, ct)));

    private static Task<IResult> Grupos(ClaimsPrincipal usuario, ConviteServico servico, CancellationToken ct) =>
        ComConta(usuario, async conta => Results.Ok(await servico.ListarGruposAsync(conta, ct)));

    private static Task<IResult> CriarGrupo(GrupoPedido pedido, ClaimsPrincipal usuario, ConviteServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.CriarGrupoAsync(conta, pedido.Nome, ct)));

    private static Task<IResult> Convites(
        Guid id,
        ClaimsPrincipal usuario,
        ConviteServico servico,
        string? nome,
        string? situacao,
        string? telefone,
        bool? transfer,
        Guid? grupoId,
        string? observacao,
        bool? somenteConfirmados,
        CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.ListarAsync(
            conta,
            id,
            new FiltroConvites(nome, EnumOuNulo<SituacaoConvidado>(situacao), telefone, transfer, grupoId, observacao, somenteConfirmados == true),
            ct)));

    private static Task<IResult> CriarConvite(Guid id, NovoConvite pedido, ClaimsPrincipal usuario, ConviteServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.CriarAsync(conta, id, pedido, ct)));

    private static Task<IResult> AtualizarConvite(Guid id, NovoConvite pedido, ClaimsPrincipal usuario, ConviteServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.AtualizarAsync(conta, id, pedido, ct)));

    private static Task<IResult> CriarConvidado(Guid id, ConvidadoPedido pedido, ClaimsPrincipal usuario, ConviteServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.SalvarConvidadoAsync(conta, id, null, pedido, ct)));

    private static Task<IResult> AtualizarConvidado(
        Guid id,
        Guid convidadoId,
        ConvidadoPedido pedido,
        ClaimsPrincipal usuario,
        ConviteServico servico,
        CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.SalvarConvidadoAsync(conta, id, convidadoId, pedido, ct)));

    private static Task<IResult> RemoverConvidado(Guid id, ClaimsPrincipal usuario, ConviteServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.RemoverConvidadoAsync(conta, id, ct)));

    private static Task<IResult> Principal(Guid id, PrincipalPedido pedido, ClaimsPrincipal usuario, ConviteServico servico, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.DefinirPrincipalAsync(conta, id, pedido.ConvidadoId, ct)));

    private static Task<IResult> Planilha(Guid id, ClaimsPrincipal usuario, ConviteServico servico, CancellationToken ct) =>
        ComConta(usuario, async conta =>
        {
            if (!await EventoDaConta(conta, id, ct, servico))
            {
                return Indisponivel();
            }

            return Results.File(
                servico.ModeloPlanilha(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "convidados.xlsx");
        });

    private static Task<IResult> Importar(
        Guid id,
        IFormFile? arquivo,
        ClaimsPrincipal usuario,
        ConviteServico servico,
        CancellationToken ct) =>
        ComConta(usuario, async conta =>
        {
            if (arquivo is null)
            {
                return Validacao("Envie uma planilha .xlsx ou .xls.");
            }

            await using var fluxo = arquivo.OpenReadStream();
            return await De(servico.ImportarAsync(conta, id, fluxo, arquivo.FileName, ct));
        });

    private static Task<IResult> Qr(Guid id, ClaimsPrincipal usuario, ConviteServico servico, IConfiguration configuracao, CancellationToken ct) =>
        ComConta(usuario, async conta =>
        {
            var png = await servico.QrAsync(conta, id, configuracao["PublicUrl"] ?? "http://localhost:4200", ct);
            if (!png.Ok)
            {
                return Falha(png.Falha!);
            }

            return Results.File(png.Valor!, "image/png", "convite.png");
        });

    private static Task<IResult> WhatsApp(Guid id, ClaimsPrincipal usuario, ConviteServico servico, IConfiguration configuracao, CancellationToken ct) =>
        ComConta(usuario, conta => De(servico.WhatsAppAsync(conta, id, configuracao["PublicUrl"] ?? "http://localhost:4200", ct)));

    private static async Task<IResult> Arquivo(Guid id, ClaimsPrincipal usuario, HttpContext http, IArmazenamentoArquivo armazenamento, CancellationToken ct)
    {
        var conta = Conta(usuario);
        if (conta is null)
        {
            return NaoAutenticado();
        }

        var aberto = await armazenamento.AbrirAsync(id, conta, ct);
        if (aberto is null)
        {
            return Indisponivel();
        }

        http.Response.Headers.XContentTypeOptions = "nosniff";
        return Results.File(aberto.Conteudo, aberto.TipoConteudo, LimparNome(aberto.NomeOriginal));
    }

    private static async Task<IResult> Cep(string cep, ClaimsPrincipal usuario, IConsultaCep consulta, CancellationToken ct)
    {
        if (Conta(usuario) is null)
        {
            return NaoAutenticado();
        }

        var digitos = Mascaras.SoDigitos(cep);
        var endereco = digitos is null ? null : await consulta.ConsultarAsync(digitos, ct);
        return endereco is null
            ? Results.Json(new ErroResposta(Codigos.Validacao, "CEP não encontrado."), statusCode: StatusCodes.Status404NotFound)
            : Results.Ok(endereco);
    }

    private static Task<IResult> AbrirRsvp(string token, ConviteServico servico, CancellationToken ct) =>
        De(servico.AbrirRsvpAsync(token, ct));

    private static Task<IResult> ResponderRsvp(string token, RespostasPedido pedido, ConviteServico servico, CancellationToken ct) =>
        De(servico.ResponderAsync(token, pedido.Respostas, ct));

    private static async Task<IResult> Imagem(
        string conta,
        IFormFile? arquivo,
        IArmazenamentoArquivo armazenamento,
        Func<Guid, CancellationToken, Task<(bool Ok, Falha? Falha, Guid? Anterior)>> ligar,
        CancellationToken ct)
    {
        if (arquivo is null || arquivo.Length == 0)
        {
            return Validacao("A imagem não foi aceita. Use JPG, PNG ou WebP de até 5 MB.");
        }

        await using var fluxo = arquivo.OpenReadStream();
        var guardado = await armazenamento.GuardarImagemAsync(fluxo, arquivo.FileName, conta, ct);
        if (!guardado.Ok || guardado.Valor is null)
        {
            return Falha(guardado.Falha!);
        }

        var imagemId = guardado.Valor.Id;
        try
        {
            var ligado = await ligar(imagemId, ct);
            if (!ligado.Ok)
            {
                await armazenamento.DescartarAsync(imagemId, ct);
                return Falha(ligado.Falha!);
            }

            if (ligado.Anterior is Guid anterior && anterior != imagemId)
            {
                await armazenamento.DescartarAsync(anterior, ct);
            }

            return Results.Ok(new { imagemId, mensagem = "Imagem gravada." });
        }
        catch
        {
            await armazenamento.DescartarAsync(imagemId, ct);
            throw;
        }
    }

    private static async Task<bool> EventoDaConta(string conta, Guid id, CancellationToken ct, ConviteServico servico)
    {
        var lista = await servico.ListarAsync(conta, id, new FiltroConvites(null, null, null, null, null, null, false), ct);
        return lista.Ok;
    }

    private static Task<IResult> ComConta(ClaimsPrincipal usuario, Func<string, Task<IResult>> acao)
    {
        var conta = Conta(usuario);
        return conta is null ? Task.FromResult(NaoAutenticado()) : acao(conta);
    }

    private static async Task<IResult> De<T>(Task<Resultado<T>> tarefa)
    {
        var resultado = await tarefa;
        return resultado.Ok ? Results.Ok(resultado.Valor) : Falha(resultado.Falha!);
    }

    private static IResult Falha(Falha falha) =>
        Results.Json(
            new ErroResposta(falha.Codigo, falha.Mensagem),
            statusCode: falha.Codigo == Codigos.Indisponivel ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest);

    private static IResult Validacao(string mensagem) => Falha(new Falha(Codigos.Validacao, mensagem));

    private static IResult Indisponivel() => Falha(new Falha(Codigos.Indisponivel, "Indisponível."));

    private static IResult NaoAutenticado() =>
        Results.Json(new ErroResposta("autenticacao", "É preciso entrar para continuar."), statusCode: StatusCodes.Status401Unauthorized);

    private static string? Conta(ClaimsPrincipal usuario) => usuario.FindFirstValue(ClaimTypes.NameIdentifier);

    private static string Email(ClaimsPrincipal usuario) =>
        usuario.FindFirstValue(ClaimTypes.Email) ?? usuario.FindFirstValue("email") ?? "";

    private static DateOnly Hoje() => DateOnly.FromDateTime(DateTime.UtcNow);

    private static T? EnumOuNulo<T>(string? valor)
        where T : struct, Enum =>
        Enum.TryParse<T>(valor, true, out var lido) && Enum.IsDefined(lido) ? lido : null;

    private static string LimparNome(string nome)
    {
        var limpo = Path.GetFileName(nome).Replace("\"", "", StringComparison.Ordinal);
        return string.IsNullOrWhiteSpace(limpo) ? "arquivo" : limpo;
    }
}

public sealed record TransferPedido(bool Ativo, IReadOnlyList<string>? Locais);

public sealed record VinculoPedido(Guid ClienteId);

public sealed record GrupoPedido(string? Nome);

public sealed record PrincipalPedido(Guid ConvidadoId);

public sealed record RespostasPedido(IReadOnlyList<RespostaRsvp>? Respostas);
