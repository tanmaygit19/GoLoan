using GoLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class DisbursementDTO2
    {
        public int DisbursementId { get; set; }
        public int DealId { get; set; }
        public decimal DisburseAmount { get; set; }
        public string BankPartner { get; set; }
        public DateTime DisbursementDate { get; set; }
        public string Status { get; set; }
    }
}
