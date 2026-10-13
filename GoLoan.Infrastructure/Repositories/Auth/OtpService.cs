using GoLoan.Application.Interfaces.Auth;
using OtpNet;
using QRCoder;

namespace GoLoan.Infrastructure.Repositories.Auth
{
    public class OtpService : IOtpService
    {
        public string GenerateSecret()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        public string GenerateQrCodeBase64(string secret, string email)
        {
            var otpAuthUri = $"otpauth://totp/GoLoan:{email}?secret={secret}&issuer=GoLoan&digits=6&period=30";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(otpAuthUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var pngBytes = qrCode.GetGraphic(5);
            return Convert.ToBase64String(pngBytes);
        }

        public bool VerifyTotp(string secret, string code)
        {
            var keyBytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(keyBytes);
            return totp.VerifyTotp(code.Trim(), out _, new VerificationWindow(previous: 1, future: 1));
        }
    }
}
