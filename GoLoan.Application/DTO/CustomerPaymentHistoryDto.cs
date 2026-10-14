using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class CustomerPaymentHistoryDto
    {
        public DateTime Date { get; set; }
        public string LoanAccountNo { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMode { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public string PaymentType { get; set; }
    }
}
