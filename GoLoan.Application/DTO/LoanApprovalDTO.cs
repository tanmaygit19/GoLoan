using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class LoanApprovalDTO
    {
        public int DealId { get; set; }

        public decimal ApprovedAmount { get; set; }

        public string? RejectionReason { get; set; }
    }
}
