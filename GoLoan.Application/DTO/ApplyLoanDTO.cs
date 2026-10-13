using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GoLoan.Application.DTO
{
    public  class ApplyLoanDTO
    {
        public int CustomerId { get; set; }
        public string LoanType { get; set; }

        [Range(1, 100000000)]
        public decimal LoanAmount { get; set; }

        [Range(1, 360)]
        public int TenureMonths { get; set; }

        [Required]
        public string BankName { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{9,18}$")]
        public string BankAccountNumber { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$")]
        public string IFSCCode { get; set; }

        [Range(1, 31)]
        public int EmiDay { get; set; }
    }
}
