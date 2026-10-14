using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class ForeClosureHistoryDto
    {

        public int RequestId { get; set; }
        public int LoanAccountId { get; set; }
        public string ForeClosureType { get; set; }
        public decimal ForeClosureAmount { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ExpectedClosureDate { get; set; }
        public string Status { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public int? NoOfEmi { get; set; }
        public string? RazorpayOrderId { get; set; }
    }
}
