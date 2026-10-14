using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class LoanStatusDTO
    {
        public int DealId { get; set; }
        public string LoanType { get; set; }
        public decimal LoanAmount { get; set; }
        public int TenureMonths { get; set; }
        public string? CurrentStatus { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public string? RejectionReason { get; set; }
    }
}
