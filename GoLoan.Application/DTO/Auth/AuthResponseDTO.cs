namespace GoLoan.Application.DTO.Auth
{
    public class AuthResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? TokenType { get; set; }
        public bool RequiresTwoFactor { get; set; } = false;
        public bool RequiresTwoFactorSetup { get; set; } = false;
        public string? PreAuthToken { get; set; }
        public string? QrCodeBase64 { get; set; }
        public string? TempSecret { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int? CustomerId { get; set; }
    }
}
