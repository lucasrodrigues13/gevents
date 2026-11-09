using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
using ExcelDataReader;
using Project.Application.Convites;
using Project.Domain;
using Project.Domain.Convites;

namespace Project.Infrastructure.Convites;

public sealed class LeitorPlanilhaConvite : ILeitorPlanilha
{
    public LeitorPlanilhaConvite()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public IReadOnlyList<IReadOnlyList<string>> Ler(Stream conteudo, string? nomeArquivo)
    {
        var extensao = Path.GetExtension(nomeArquivo ?? "");
        if (!extensao.Equals(".xlsx", StringComparison.OrdinalIgnoreCase)
            && !extensao.Equals(".xls", StringComparison.OrdinalIgnoreCase))
        {
            throw new RegraDeNegocioException("A planilha não é aceita.");
        }

        try
        {
            if (conteudo.CanSeek)
            {
                conteudo.Position = 0;
            }

            using var leitor = ExcelReaderFactory.CreateReader(conteudo);
            var linhas = new List<IReadOnlyList<string>>();
            do
            {
                while (leitor.Read())
                {
                    var colunas = new string[Math.Max(leitor.FieldCount, ImportacaoConvites.Cabecalho.Length)];
                    for (var indice = 0; indice < leitor.FieldCount; indice++)
                    {
                        colunas[indice] = Convert.ToString(leitor.GetValue(indice), CultureInfo.InvariantCulture)?.Trim() ?? "";
                    }

                    linhas.Add(colunas);
                }
            }
            while (leitor.NextResult());

            return linhas;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw new RegraDeNegocioException("A planilha não é aceita.");
        }
    }

    public byte[] Modelo()
    {
        return Criar([ImportacaoConvites.Cabecalho]);
    }

    public byte[] Criar(IReadOnlyList<IReadOnlyList<string>> linhas)
    {
        using var memoria = new MemoryStream();
        using (var zip = new ZipArchive(memoria, ZipArchiveMode.Create, leaveOpen: true))
        {
            Escrever(zip, "[Content_Types].xml", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
                  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
                  <Default Extension="xml" ContentType="application/xml"/>
                  <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
                  <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
                </Types>
                """);
            Escrever(zip, "_rels/.rels", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
                </Relationships>
                """);
            Escrever(zip, "xl/workbook.xml", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
                  <sheets>
                    <sheet name="Convidados" sheetId="1" r:id="rId1"/>
                  </sheets>
                </workbook>
                """);
            Escrever(zip, "xl/_rels/workbook.xml.rels", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
                </Relationships>
                """);
            Escrever(zip, "xl/worksheets/sheet1.xml", Planilha(linhas));
        }

        return memoria.ToArray();
    }

    private static string Planilha(IReadOnlyList<IReadOnlyList<string>> linhas)
    {
        var ns = XNamespace.Get("http://schemas.openxmlformats.org/spreadsheetml/2006/main");
        var sheetData = new XElement(ns + "sheetData");
        for (var linha = 0; linha < linhas.Count; linha++)
        {
            var row = new XElement(ns + "row", new XAttribute("r", linha + 1));
            for (var coluna = 0; coluna < linhas[linha].Count; coluna++)
            {
                var referencia = Coluna(coluna) + (linha + 1);
                row.Add(new XElement(
                    ns + "c",
                    new XAttribute("r", referencia),
                    new XAttribute("t", "inlineStr"),
                    new XElement(ns + "is", new XElement(ns + "t", linhas[linha][coluna] ?? ""))));
            }

            sheetData.Add(row);
        }

        var documento = new XDocument(new XElement(ns + "worksheet", sheetData));
        return documento.ToString(SaveOptions.DisableFormatting);
    }

    private static string Coluna(int indice)
    {
        var nome = "";
        var numero = indice + 1;
        while (numero > 0)
        {
            numero--;
            nome = (char)('A' + numero % 26) + nome;
            numero /= 26;
        }

        return nome;
    }

    private static void Escrever(ZipArchive zip, string caminho, string conteudo)
    {
        var entrada = zip.CreateEntry(caminho, CompressionLevel.Fastest);
        using var saida = entrada.Open();
        using var escritor = new StreamWriter(saida, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        escritor.Write(conteudo);
    }
}
