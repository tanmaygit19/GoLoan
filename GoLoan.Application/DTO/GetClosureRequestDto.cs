using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class GetClosureRequestDto
    {
        public int RequestId { get; set; }
        public string LoanAccountNo { get; set; }
        public string CustomerName { get; set; }
        public decimal ForeClosureAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public string Status { get; set; }
    }
}
