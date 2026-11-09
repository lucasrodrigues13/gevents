using Project.Application.Arquivos;
using Project.Application.Cep;
using Project.Application.Eventos;
using Project.Domain;
using Project.Domain.Clientes;
using Project.Domain.Eventos;

namespace Project.Application.Clientes;

public sealed class ClienteServico
{
    private readonly IEventoConsulta _eventos;
    private readonly IClienteConsulta _consulta;
    private readonly IClienteRepositorio _repositorio;
    private readonly IConsultaCep _cep;
    private readonly IArmazenamentoArquivo _arquivos;

    public ClienteServico(
        IEventoConsulta eventos,
        IClienteConsulta consulta,
        IClienteRepositorio repositorio,
        IConsultaCep cep,
        IArmazenamentoArquivo arquivos)
    {
        _eventos = eventos;
        _consulta = consulta;
        _repositorio = repositorio;
        _cep = cep;
        _arquivos = arquivos;
    }

    public Task<IReadOnlyList<ClienteItem>> ListarAsync(string accountId, string? nome, CancellationToken cancellationToken)
    {
        return _consulta.ListarAsync(accountId, nome, cancellationToken);
    }

    public async Task<Cliente?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        return await _consulta.ObterAsync(accountId, id, cancellationToken);
    }

    public async Task<Guid> CriarAsync(
        string accountId,
        ClienteDados dados,
        Stream? imagem,
        string? nomeImagem,
        Guid? eventoId,
        CancellationToken cancellationToken)
    {
        if (eventoId is not null && await _eventos.ObterAsync(accountId, eventoId.Value, cancellationToken) is null)
        {
            throw new RegraDeNegocioException("O evento não está disponível.");
        }

        dados = await CompletarEnderecoAsync(dados, cancellationToken);
        var cliente = Cliente.Criar(accountId, dados);
        var (arquivoId, _) = await TentarImagemAsync(imagem, nomeImagem, cancellationToken);
        if (arquivoId is not null)
        {
            cliente.DefinirArquivo(arquivoId);
        }

        try
        {
            await _repositorio.AdicionarAsync(cliente, cancellationToken);
            if (eventoId is not null && !await _consulta.ExisteVinculoAsync(accountId, cliente.Id, eventoId.Value, cancellationToken))
            {
                await _repositorio.AdicionarAsync(VinculoCliente.Criar(accountId, cliente.Id, eventoId.Value), cancellationToken);
            }

            await _repositorio.SalvarAsync(cancellationToken);
        }
        catch
        {
            if (arquivoId is not null)
            {
                await _arquivos.ExcluirAsync(arquivoId.Value, cancellationToken);
            }

            throw;
        }

        return cliente.Id;
    }

    public async Task EditarAsync(
        string accountId,
        Guid id,
        ClienteDados dados,
        Stream? imagem,
        string? nomeImagem,
        CancellationToken cancellationToken)
    {
        var cliente = await _consulta.ObterAsync(accountId, id, cancellationToken)
            ?? throw new RegraDeNegocioException("O cliente não está disponível.");
        dados = await CompletarEnderecoAsync(dados, cancellationToken);
        cliente.Editar(dados);
        var anterior = cliente.ArquivoId;
        var (novo, _) = await TentarImagemAsync(imagem, nomeImagem, cancellationToken);
        if (novo is not null)
        {
            cliente.DefinirArquivo(novo);
        }

        try
        {
            await _repositorio.SalvarAsync(cancellationToken);
        }
        catch
        {
            if (novo is not null)
            {
                await _arquivos.ExcluirAsync(novo.Value, cancellationToken);
            }

            throw;
        }

        if (novo is not null && anterior is not null)
        {
            await _arquivos.ExcluirAsync(anterior.Value, cancellationToken);
        }

    }

    public async Task<bool> LinkExisteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _consulta.ObterLinkAsync(id, cancellationToken) is not null;
    }

    public async Task<Guid?> ObterLinkDoEventoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        var link = await _consulta.ObterLinkAsync(accountId, eventoId, cancellationToken);
        return link?.Id;
    }

    public async Task<Guid> ObterOuCriarLinkAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        if (await _eventos.ObterAsync(accountId, eventoId, cancellationToken) is null)
        {
            throw new RegraDeNegocioException("O evento não está disponível.");
        }

        var existente = await _consulta.ObterLinkAsync(accountId, eventoId, cancellationToken);
        if (existente is not null)
        {
            return existente.Id;
        }

        var link = LinkCadastroCliente.Criar(accountId, eventoId);
        await _repositorio.AdicionarAsync(link, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
        return link.Id;
    }

    public async Task<Guid> CadastrarPeloLinkAsync(
        Guid linkId,
        ClienteDados dados,
        Stream? imagem,
        string? nomeImagem,
        CancellationToken cancellationToken)
    {
        var link = await _consulta.ObterLinkAsync(linkId, cancellationToken)
            ?? throw new RegraDeNegocioException("O link não está disponível.");
        return await CriarAsync(link.AccountId, dados, imagem, nomeImagem, link.EventoId, cancellationToken);
    }

    public async Task<ArquivoBaixado?> AbrirImagemAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        var cliente = await _consulta.ObterAsync(accountId, id, cancellationToken);
        if (cliente?.ArquivoId is null)
        {
            return null;
        }

        return await _arquivos.AbrirAsync(cliente.ArquivoId.Value, cancellationToken);
    }

    public async Task VincularAsync(string accountId, Guid clienteId, Guid eventoId, CancellationToken cancellationToken)
    {
        if (await _eventos.ObterAsync(accountId, eventoId, cancellationToken) is null
            || await _consulta.ObterAsync(accountId, clienteId, cancellationToken) is null)
        {
            throw new RegraDeNegocioException("O cliente não está disponível.");
        }

        if (await _consulta.ExisteVinculoAsync(accountId, clienteId, eventoId, cancellationToken))
        {
            return;
        }

        await _repositorio.AdicionarAsync(VinculoCliente.Criar(accountId, clienteId, eventoId), cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task DesvincularAsync(string accountId, Guid clienteId, Guid eventoId, CancellationToken cancellationToken)
    {
        var vinculo = await _consulta.ObterVinculoAsync(accountId, clienteId, eventoId, cancellationToken);
        if (vinculo is null)
        {
            return;
        }

        await _repositorio.RemoverAsync(vinculo, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    public Task<IReadOnlyList<ClienteResumo>> ListarDoEventoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        return _consulta.ListarDoEventoAsync(accountId, eventoId, cancellationToken);
    }

    private async Task<ClienteDados> CompletarEnderecoAsync(ClienteDados dados, CancellationToken cancellationToken)
    {
        var atual = (dados.Endereco ?? EnderecoLocal.Vazio).Normalizar();
        var digitos = atual.CepDigitos();
        if (digitos is null || digitos.Length != 8)
        {
            return dados with { Endereco = atual };
        }

        var achado = await _cep.ConsultarAsync(digitos, cancellationToken);
        if (achado is null)
        {
            return dados with { Endereco = atual };
        }

        return dados with
        {
            Endereco = atual with
            {
                Logradouro = achado.Logradouro,
                Estado = achado.Estado,
                Cidade = achado.Cidade,
                Bairro = achado.Bairro
            }
        };
    }

    private async Task<(Guid? Id, string? Aviso)> TentarImagemAsync(Stream? imagem, string? nome, CancellationToken cancellationToken)
    {
        if (imagem is null)
        {
            return (null, null);
        }

        try
        {
            var id = await _arquivos.SalvarImagemAsync(imagem, nome ?? "imagem", cancellationToken);
            return (id, null);
        }
        catch (RegraDeNegocioException exception)
        {
            return (null, exception.Message);
        }
    }
}
