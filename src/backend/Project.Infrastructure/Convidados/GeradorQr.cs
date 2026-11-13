using Project.Application.Convidados;
using QRCoder;

namespace Project.Infrastructure.Convidados;

public sealed class GeradorQr : IGeradorQr
{
    public byte[] Png(string texto)
    {
        using var gerador = new QRCodeGenerator();
        using var dados = gerador.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
        var png = new PngByteQRCode(dados);
        return png.GetGraphic(8);
    }
}
