namespace GoLoan.Application.DTO.Auth
{
    public class EmailSettingsDTO
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public string SenderName { get; set; } = "GoLoan";
        public bool EnableSsl { get; set; } = true;
    }
}
