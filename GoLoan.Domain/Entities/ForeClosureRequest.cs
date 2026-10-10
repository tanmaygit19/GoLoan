using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoLoan.Domain.Entities
{
    public class ForeClosureRequest
    {
        [Key]
        public int RequestId { get; set; }

        [ForeignKey("LoanAccount")]
        public int LoanAccountId { get; set; }

        [Required]
        public string ForeClosureType { get; set; }

        public decimal ForeClosureAmount { get; set; }

        public int? NoOfEmi { get; set; }

        public DateTime RequestedDate { get; set; }

        [Required]
        public DateTime ExpectedClosureDate { get; set; }

        [Required]
        public string Reason { get; set; }

        public string Status { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime? PaidDate { get; set; }
        public string? RazorpayOrderId { get; set; }

        [ForeignKey("User")]
        public int? ClosedBy { get; set; }

        public LoanAccount? LoanAccount { get; set; }
    }
}