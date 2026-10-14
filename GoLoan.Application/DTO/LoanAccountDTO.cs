using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class LoanAccountDTO
    {
        public int LoanAccountId { get; set; }

        public int DealId { get; set; }

        public int CustomerId { get; set; }

        public string LoanAccountNo { get; set; } = string.Empty;

        public decimal LoanAmount { get; set; }

        public decimal OutstandingPrincipal { get; set; }

        public string LoanStatus { get; set; } = string.Empty;

        public decimal InterestRate { get; set; }

        public int TenureMonths { get; set; }

        public decimal EmiAmount { get; set; }

        public DateTime DisbursementDate { get; set; }

        public decimal TotalPaidAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
