using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class SanctionLetterDTO
    {

            public int SanctionId { get; set; }
            public int DealId { get; set; }
            public string LoanType { get; set; }
            public decimal LoanAmount { get; set; }
            public double InterestRate { get; set; }
            public int TenureMonths { get; set; }
            public decimal EmiAmount { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }

