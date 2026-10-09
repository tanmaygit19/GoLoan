using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace GoLoan.Domain.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public bool IsTwoFaEnabled { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public string? GoogleSubjectId { get; set; }
        public string AuthProvider { get; set; } = "Local";
        public Role? Role { get; set; }
        public Customer? Customer { get; set; }
        public List<DealReview>? DealReviews { get; set; }
        public List<OtpRecord>? OtpRecords { get; set; }
    }
}
