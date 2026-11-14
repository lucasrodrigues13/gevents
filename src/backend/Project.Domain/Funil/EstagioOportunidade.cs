namespace Project.Domain.Funil;

public enum EstagioOportunidade
{
    ContatoInicial = 0,
    OrcamentoEnviado = 1,
    Contratado = 2,
    Descartado = 3,
}

public static class RotulosEstagio
{
    public static string Texto(EstagioOportunidade estagio) => estagio switch
    {
        EstagioOportunidade.ContatoInicial => "Contato inicial",
        EstagioOportunidade.OrcamentoEnviado => "Orçamento enviado",
        EstagioOportunidade.Contratado => "Contratado",
        EstagioOportunidade.Descartado => "Descartado",
        _ => estagio.ToString(),
    };

    public static readonly EstagioOportunidade[] Todos =
    [
        EstagioOportunidade.ContatoInicial,
        EstagioOportunidade.OrcamentoEnviado,
        EstagioOportunidade.Contratado,
        EstagioOportunidade.Descartado,
    ];
}
