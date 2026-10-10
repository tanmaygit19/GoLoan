using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class ForeClosureRequestDto
    {
        public int LoanAccountId { get; set; }

        public string ForeClosureType { get; set; }

        public int? NoOfEmi { get; set; }

        public string Reason { get; set; }

    }
}
