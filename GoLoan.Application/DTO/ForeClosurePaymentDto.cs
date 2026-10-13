using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class ForeClosurePaymentDto
    {

        public int RequestId { get; set; }
        public int LoanAccountId { get; set; }
        public string ForeClosureType { get; set; }
        public decimal ForeClosureAmount { get; set; }
        public string Status { get; set; }
        public DateTime PaidDate { get; set; }


    }
}
