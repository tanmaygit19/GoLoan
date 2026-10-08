using System.ComponentModel.DataAnnotations;

namespace GoLoan.Domain.Entities
{
    public class OtpVM
    {
        public string Email { get; set; }

        [Required]
        public string OTP { get; set; }
    }
}