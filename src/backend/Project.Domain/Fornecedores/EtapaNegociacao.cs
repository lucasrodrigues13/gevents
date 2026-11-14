namespace Project.Domain.Fornecedores;

public enum EtapaNegociacao
{
    Recomendado = 0,
    OrcamentoSolicitado = 1,
    OrcamentoRecebido = 2,
    Contratado = 3,
}

public enum SituacaoParcela
{
    Pendente = 0,
    Pago = 1,
}
