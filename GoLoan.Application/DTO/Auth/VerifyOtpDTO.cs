using System.ComponentModel.DataAnnotations;

namespace GoLoan.Application.DTO.Auth
{
    public class VerifyOtpDTO
    {
        [Required(ErrorMessage = "Code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be 6 digits")]
        public string Code { get; set; } = string.Empty;
    }
}
