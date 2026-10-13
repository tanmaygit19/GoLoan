using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class DisbursementDTO
    {

        public int DealId { get; set; }
        public decimal DisburseAmount { get; set; }
        public string BankPartner { get; set; }

    }
}
