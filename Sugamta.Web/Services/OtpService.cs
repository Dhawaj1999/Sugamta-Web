using QRCoder;

namespace Sugamta.Web.Services
{
    public class OtpService
    {
        public string GenerateOtp()
        {
            var generator = new Random();
            return generator.Next(100000, 999999).ToString();
        }

        public byte[] GenerateQrCode(string otp)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(otp, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(10);
        }
    }
}
