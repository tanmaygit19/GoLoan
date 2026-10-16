using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class GetForeClosureRequestAdminDto
    {

        public int RequestId { get; set; }
        public string? LoanAccountNo { get; set; }
        public string? CustomerName { get; set; }
        public string? Type { get; set; }
        public DateTime RequestedDate { get; set; }
        public decimal OutstandingPrincipal { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }

    }
}
