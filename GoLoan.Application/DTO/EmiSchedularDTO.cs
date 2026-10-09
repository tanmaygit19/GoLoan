using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class EmiSchedularDTO
    {
        public int EmiScheduleId { get; set; }
        public int LoanAccountId { get; set; }
        public int InstallmentNo { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Emi { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime? PaidDate { get; set; }
    }
}
