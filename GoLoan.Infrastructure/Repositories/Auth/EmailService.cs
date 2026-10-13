using System.Net;
using System.Net.Mail;
using GoLoan.Application.DTO.Auth;
using GoLoan.Application.Interfaces.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GoLoan.Infrastructure.Repositories.Auth
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettingsDTO _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettingsDTO> options, ILogger<EmailService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<bool> SendOtpEmail(string toEmail, string userName, string otpCode)
        {
            try
            {
                if (string.IsNullOrEmpty(_settings.SenderEmail) || string.IsNullOrEmpty(_settings.SenderPassword))
                {
                    _logger.LogInformation("================================================");
                    _logger.LogInformation("TEST OTP for {Email} ({Name}): {Code}", toEmail, userName, otpCode);
                    _logger.LogInformation("================================================");
                    return true;
                }

                using var smtp = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword),
                    EnableSsl = _settings.EnableSsl
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                    Subject = "GoLoan Verification Code",
                    Body = $"Hello {userName}, your verification code is: {otpCode}. Valid for 5 minutes.",
                    IsBodyHtml = false
                };
                mail.To.Add(toEmail);

                await smtp.SendMailAsync(mail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }
    }
}
