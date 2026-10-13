namespace GoLoan.Application.Interfaces.Auth
{
    public interface IOtpService
    {
        string GenerateSecret();
        string GenerateQrCodeBase64(string secret, string email);
        bool VerifyTotp(string secret, string code);
    }
}
