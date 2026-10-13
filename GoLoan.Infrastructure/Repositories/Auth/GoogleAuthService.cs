using Google.Apis.Auth;
using GoLoan.Application.Interfaces.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GoLoan.Infrastructure.Repositories.Auth
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(IConfiguration configuration, ILogger<GoogleAuthService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<GoogleUserInfo?> ValidateGoogleToken(string idToken)
        {
            try
            {
                var clientId = _configuration["GoogleAuthSettings:ClientId"];

                var settings = new GoogleJsonWebSignature.ValidationSettings();

                if (!string.IsNullOrWhiteSpace(clientId) && !clientId.Contains("YOUR_GOOGLE_CLIENT_ID"))
                {
                    settings.Audience = new[] { clientId };
                }

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return new GoogleUserInfo
                {
                    Email = payload.Email,
                    FirstName = payload.GivenName ?? payload.Name ?? "Google",
                    LastName = payload.FamilyName ?? "User",
                    GoogleSubjectId = payload.Subject,
                    EmailVerified = payload.EmailVerified
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google token validation failed: {Message}", ex.Message);
                return null;
            }
        }
    }
}
