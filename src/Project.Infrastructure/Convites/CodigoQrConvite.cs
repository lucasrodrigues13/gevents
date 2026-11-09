using Project.Application.Convites;
using QRCoder;

namespace Project.Infrastructure.Convites;

public sealed class CodigoQrConvite : ICodigoQr
{
    public byte[] Png(string texto)
    {
        using var gerador = new QRCodeGenerator();
        using var dados = gerador.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
        return new PngByteQRCode(dados).GetGraphic(4);
    }
}
