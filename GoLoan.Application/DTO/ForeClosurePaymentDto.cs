using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class ForeClosurePaymentDto
    {

        public int RequestId { get; set;}
        public string PaymentMode { get; set; }
        public string TransactionReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaidAmount { get; set; }


    }
}
