namespace GoLoan.Application.DTO.Auth
{
    public class UserProfileDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsTwoFaEnabled { get; set; }
        public string AuthProvider { get; set; } = string.Empty;
    }
}
