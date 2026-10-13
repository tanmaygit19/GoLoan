namespace GoLoan.Application.Interfaces.Auth
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo?> ValidateGoogleToken(string idToken);
    }

    public class GoogleUserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string GoogleSubjectId { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }
    }
}
