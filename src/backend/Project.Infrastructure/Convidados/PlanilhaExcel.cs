using System.Text;
using ExcelDataReader;
using Project.Application.Comum;
using Project.Application.Convidados;
using Project.Domain.Convidados;

namespace Project.Infrastructure.Convidados;

public sealed class PlanilhaExcel : IPlanilhaConvites
{
    public PlanilhaExcel()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public byte[] Modelo()
    {
        using var livro = new ClosedXML.Excel.XLWorkbook();
        var aba = livro.AddWorksheet("Convidados");
        for (var i = 0; i < PlanilhaConvites.Colunas.Length; i++)
        {
            aba.Cell(1, i + 1).Value = PlanilhaConvites.Colunas[i];
        }

        using var memoria = new MemoryStream();
        livro.SaveAs(memoria);
        return memoria.ToArray();
    }

    public Resultado<IReadOnlyList<LinhaPlanilha>> Ler(Stream arquivo, string? nomeArquivo)
    {
        var extensao = Path.GetExtension(nomeArquivo ?? "").ToLowerInvariant();
        if (extensao is not ".xlsx" and not ".xls")
        {
            return Resultado<IReadOnlyList<LinhaPlanilha>>.Erro(Codigos.Validacao, "Envie uma planilha .xlsx ou .xls.");
        }

        try
        {
            using var leitor = ExcelReaderFactory.CreateReader(arquivo);
            if (!leitor.Read())
            {
                return Resultado<IReadOnlyList<LinhaPlanilha>>.Erro(Codigos.Validacao, "A planilha não tem as colunas do modelo.");
            }

            var cabecalho = new string?[PlanilhaConvites.Colunas.Length];
            for (var i = 0; i < cabecalho.Length; i++)
            {
                cabecalho[i] = i < leitor.FieldCount ? leitor.GetValue(i)?.ToString() : null;
            }

            if (!PlanilhaConvites.CabecalhoValido(cabecalho))
            {
                return Resultado<IReadOnlyList<LinhaPlanilha>>.Erro(Codigos.Validacao, "A planilha não tem as colunas do modelo.");
            }

            var linhas = new List<LinhaPlanilha>();
            while (leitor.Read())
            {
                string? Celula(int indice) => indice < leitor.FieldCount ? leitor.GetValue(indice)?.ToString() : null;
                if (Enumerable.Range(0, PlanilhaConvites.Colunas.Length).All(indice => string.IsNullOrWhiteSpace(Celula(indice))))
                {
                    continue;
                }

                linhas.Add(new LinhaPlanilha(
                    Celula(0), Celula(1), Celula(2), Celula(3), Celula(4), Celula(5),
                    Celula(6), Celula(7), Celula(8), Celula(9), Celula(10)));
            }

            return Resultado<IReadOnlyList<LinhaPlanilha>>.Sucesso(linhas);
        }
        catch (Exception)
        {
            return Resultado<IReadOnlyList<LinhaPlanilha>>.Erro(Codigos.Validacao, "A planilha não pôde ser importada.");
        }
    }
}
