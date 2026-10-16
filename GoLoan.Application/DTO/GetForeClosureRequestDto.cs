using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class GetForeClosureRequestDto
    {
        public int RequestId { get; set; }
        public string LoanAccountNo { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public int? NoOfEmi { get; set; }

    }
}
