using Project.Application.Comum;
using Project.Domain.Clientes;

namespace Project.Application.Negocio;

public sealed record NovoCliente(
    string? Nome,
    PapelCliente? Papel,
    DateOnly? Nascimento,
    string? Email,
    string? Ddi,
    string? Celular,
    string? TelefoneFixo,
    string? Cpf,
    string? Rg,
    string? Cnpj,
    string? Profissao,
    string? InscricaoMunicipal,
    string? Religiao,
    string? Observacao,
    string? Facebook,
    string? Instagram,
    string? Pais,
    string? Cep,
    string? Endereco,
    string? Numero,
    string? Estado,
    string? Cidade,
    string? Bairro,
    string? Complemento);

public sealed record ClienteLeitura(
    Guid Id,
    string Nome,
    PapelCliente Papel,
    DateOnly? Nascimento,
    string Email,
    string Ddi,
    string Celular,
    string? TelefoneFixo,
    string? Cpf,
    string? Rg,
    string? Cnpj,
    string? Profissao,
    string? InscricaoMunicipal,
    string? Religiao,
    string? Observacao,
    string? Facebook,
    string? Instagram,
    string? Pais,
    string? Cep,
    string? Endereco,
    string? Numero,
    string? Estado,
    string? Cidade,
    string? Bairro,
    string? Complemento,
    Guid? FotoId,
    string? UltimoAcesso,
    IReadOnlyList<EventoVinculado> Eventos);

public sealed record EventoVinculado(Guid Id, string Nome);

public sealed class ClienteServico(IDados dados, Cep.IConsultaCep cep)
{
    public async Task<IReadOnlyList<ClienteLeitura>> ListarAsync(string contaId, string? nome, CancellationToken ct)
    {
        var clientes = await dados.ListarClientesAsync(contaId, nome, ct);
        var lista = new List<ClienteLeitura>();
        foreach (var cliente in clientes)
        {
            lista.Add(await LerAsync(contaId, cliente, ct));
        }

        return lista;
    }

    public async Task<Resultado<ClienteLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var cliente = await dados.ObterClienteAsync(contaId, id, ct);
        if (cliente is null)
        {
            return Resultado<ClienteLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        return Resultado<ClienteLeitura>.Sucesso(await LerAsync(contaId, cliente, ct));
    }

    public async Task<Resultado<ClienteLeitura>> CriarAsync(string contaId, NovoCliente? pedido, DateOnly hoje, CancellationToken ct)
    {
        var montado = Montar(contaId, pedido, hoje);
        if (!montado.Ok)
        {
            return Resultado<ClienteLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        var cepOk = await CompletarCepAsync(montado.Valor!, ct);
        if (!cepOk.Ok)
        {
            return Resultado<ClienteLeitura>.Erro(cepOk.Falha!.Codigo, cepOk.Falha.Mensagem);
        }

        await dados.InserirClienteAsync(montado.Valor!, ct);
        return Resultado<ClienteLeitura>.Sucesso(await LerAsync(contaId, montado.Valor!, ct));
    }

    public async Task<Resultado<ClienteLeitura>> AtualizarAsync(
        string contaId,
        Guid id,
        NovoCliente? pedido,
        DateOnly hoje,
        CancellationToken ct)
    {
        var cliente = await dados.ObterClienteAsync(contaId, id, ct);
        if (cliente is null)
        {
            return Resultado<ClienteLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var montado = Montar(contaId, pedido, hoje);
        if (!montado.Ok)
        {
            return Resultado<ClienteLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        var origem = montado.Valor!;
        var cepOk = await CompletarCepAsync(origem, ct);
        if (!cepOk.Ok)
        {
            return Resultado<ClienteLeitura>.Erro(cepOk.Falha!.Codigo, cepOk.Falha.Mensagem);
        }

        cliente.Nome = origem.Nome;
        cliente.Papel = origem.Papel;
        cliente.Nascimento = origem.Nascimento;
        cliente.Email = origem.Email;
        cliente.Ddi = origem.Ddi;
        cliente.Celular = origem.Celular;
        cliente.TelefoneFixo = origem.TelefoneFixo;
        cliente.Cpf = origem.Cpf;
        cliente.Rg = origem.Rg;
        cliente.Cnpj = origem.Cnpj;
        cliente.Profissao = origem.Profissao;
        cliente.InscricaoMunicipal = origem.InscricaoMunicipal;
        cliente.Religiao = origem.Religiao;
        cliente.Observacao = origem.Observacao;
        cliente.Facebook = origem.Facebook;
        cliente.Instagram = origem.Instagram;
        cliente.Pais = origem.Pais;
        cliente.Cep = origem.Cep;
        cliente.Endereco = origem.Endereco;
        cliente.Numero = origem.Numero;
        cliente.Estado = origem.Estado;
        cliente.Cidade = origem.Cidade;
        cliente.Bairro = origem.Bairro;
        cliente.Complemento = origem.Complemento;
        await dados.SalvarClienteAsync(ct);
        return Resultado<ClienteLeitura>.Sucesso(await LerAsync(contaId, cliente, ct));
    }

    public async Task<Resultado<bool>> VincularAsync(string contaId, Guid eventoId, Guid clienteId, CancellationToken ct)
    {
        var vinculado = await dados.VincularAsync(contaId, clienteId, eventoId, ct);
        if (!vinculado)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        return Resultado<bool>.Sucesso(true);
    }

    public async Task<Resultado<bool>> DesvincularAsync(string contaId, Guid eventoId, Guid clienteId, CancellationToken ct)
    {
        var ok = await dados.DesvincularAsync(contaId, clienteId, eventoId, ct);
        if (!ok)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        return Resultado<bool>.Sucesso(true);
    }

    public async Task<IReadOnlyList<ClienteLeitura>> DoEventoAsync(string contaId, Guid eventoId, CancellationToken ct)
    {
        if (!await dados.EventoDaContaAsync(contaId, eventoId, ct))
        {
            return [];
        }

        var clientes = await dados.ClientesDoEventoAsync(contaId, eventoId, ct);
        var lista = new List<ClienteLeitura>();
        foreach (var cliente in clientes)
        {
            lista.Add(await LerAsync(contaId, cliente, ct));
        }

        return lista;
    }

    public async Task<Resultado<Guid?>> GuardarFotoAsync(string contaId, Guid id, Guid fotoId, CancellationToken ct)
    {
        var cliente = await dados.ObterClienteAsync(contaId, id, ct);
        if (cliente is null)
        {
            return Resultado<Guid?>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var anterior = cliente.FotoId;
        cliente.FotoId = fotoId;
        await dados.SalvarClienteAsync(ct);
        return Resultado<Guid?>.Sucesso(anterior);
    }

    public static Resultado<Cliente> Montar(string contaId, NovoCliente? pedido, DateOnly hoje)
    {
        if (pedido is null || Domain.Comum.Texto.Vazio(pedido.Nome))
        {
            return Resultado<Cliente>.Erro(Codigos.Validacao, "Informe o nome.");
        }

        if (pedido.Papel is not PapelCliente papel || !Enum.IsDefined(papel))
        {
            return Resultado<Cliente>.Erro(Codigos.Validacao, "Escolha o papel do cliente.");
        }

        if (!Domain.Comum.Texto.EmailValido(pedido.Email))
        {
            return Resultado<Cliente>.Erro(Codigos.Validacao, "Informe um e-mail válido.");
        }

        if (Domain.Comum.Texto.Vazio(pedido.Celular))
        {
            return Resultado<Cliente>.Erro(Codigos.Validacao, "Informe o celular.");
        }

        if (pedido.Nascimento is DateOnly nascimento && nascimento > hoje)
        {
            return Resultado<Cliente>.Erro(Codigos.Validacao, "A data de nascimento não pode ser futura.");
        }

        var cpf = Mascaras.Cpf(pedido.Cpf);
        if (cpf == "")
        {
            return Resultado<Cliente>.Erro(Codigos.Validacao, "O CPF deve seguir a máscara 000.000.000-00.");
        }

        var cnpj = Mascaras.Cnpj(pedido.Cnpj);
        if (cnpj == "")
        {
            return Resultado<Cliente>.Erro(Codigos.Validacao, "O CNPJ deve seguir a máscara 00.000.000/0000-00.");
        }

        var cep = Mascaras.Cep(pedido.Cep);
        if (cep == "")
        {
            return Resultado<Cliente>.Erro(Codigos.Validacao, "O CEP deve ter 8 dígitos.");
        }

        var ddi = Domain.Comum.Texto.Limpo(pedido.Ddi) ?? "+55";
        return Resultado<Cliente>.Sucesso(new Cliente
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Nome = Domain.Comum.Texto.Limpo(pedido.Nome)!,
            Papel = papel,
            Nascimento = pedido.Nascimento,
            Email = Domain.Comum.Texto.Limpo(pedido.Email)!,
            Ddi = ddi,
            Celular = Domain.Comum.Texto.Limpo(pedido.Celular)!,
            TelefoneFixo = Domain.Comum.Texto.Limpo(pedido.TelefoneFixo),
            Cpf = cpf,
            Rg = Domain.Comum.Texto.Limpo(pedido.Rg),
            Cnpj = cnpj,
            Profissao = Domain.Comum.Texto.Limpo(pedido.Profissao),
            InscricaoMunicipal = Domain.Comum.Texto.Limpo(pedido.InscricaoMunicipal),
            Religiao = Domain.Comum.Texto.Limpo(pedido.Religiao),
            Observacao = Domain.Comum.Texto.Limpo(pedido.Observacao),
            Facebook = Domain.Comum.Texto.Limpo(pedido.Facebook),
            Instagram = Domain.Comum.Texto.Limpo(pedido.Instagram),
            Pais = Domain.Comum.Texto.Limpo(pedido.Pais),
            Cep = cep,
            Endereco = Domain.Comum.Texto.Limpo(pedido.Endereco),
            Numero = Domain.Comum.Texto.Limpo(pedido.Numero),
            Estado = Domain.Comum.Texto.Limpo(pedido.Estado),
            Cidade = Domain.Comum.Texto.Limpo(pedido.Cidade),
            Bairro = Domain.Comum.Texto.Limpo(pedido.Bairro),
            Complemento = Domain.Comum.Texto.Limpo(pedido.Complemento),
        });
    }

    private async Task<Resultado<bool>> CompletarCepAsync(Cliente cliente, CancellationToken ct)
    {
        if (cliente.Cep is null)
        {
            return Resultado<bool>.Sucesso(true);
        }

        var encontrado = await cep.ConsultarAsync(Mascaras.SoDigitos(cliente.Cep)!, ct);
        if (encontrado is null)
        {
            return Resultado<bool>.Sucesso(true);
        }

        cliente.Endereco = encontrado.Endereco;
        cliente.Estado = encontrado.Estado;
        cliente.Cidade = encontrado.Cidade;
        cliente.Bairro = encontrado.Bairro;
        return Resultado<bool>.Sucesso(true);
    }

    private async Task<ClienteLeitura> LerAsync(string contaId, Cliente cliente, CancellationToken ct)
    {
        var eventos = await dados.EventosDoClienteAsync(contaId, cliente.Id, ct);
        return new ClienteLeitura(
            cliente.Id,
            cliente.Nome,
            cliente.Papel,
            cliente.Nascimento,
            cliente.Email,
            cliente.Ddi,
            cliente.Celular,
            cliente.TelefoneFixo,
            cliente.Cpf,
            cliente.Rg,
            cliente.Cnpj,
            cliente.Profissao,
            cliente.InscricaoMunicipal,
            cliente.Religiao,
            cliente.Observacao,
            cliente.Facebook,
            cliente.Instagram,
            cliente.Pais,
            cliente.Cep,
            cliente.Endereco,
            cliente.Numero,
            cliente.Estado,
            cliente.Cidade,
            cliente.Bairro,
            cliente.Complemento,
            cliente.FotoId,
            null,
            eventos.Select(evento => new EventoVinculado(evento.Id, evento.Nome)).ToArray());
    }
}
