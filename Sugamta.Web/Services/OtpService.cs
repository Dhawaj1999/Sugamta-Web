using QRCoder;

namespace Sugamta.Web.Services
{
    public class OtpService
    {
        public string GenerateOtp()
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            string otp = new string(Enumerable.Repeat(characters, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return otp;
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
