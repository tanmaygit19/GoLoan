using System.ComponentModel.DataAnnotations;

namespace GoLoan.Application.DTO.Auth
{
    public class GoogleLoginDTO
    {
        [Required(ErrorMessage = "Google ID token is required")]
        public string IdToken { get; set; } = string.Empty;
    }
}
