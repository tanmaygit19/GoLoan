namespace GoLoan.Application.Interfaces.Auth
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmail(string toEmail, string userName, string otpCode);
    }
}
