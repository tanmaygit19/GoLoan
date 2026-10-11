using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO
{
    public class EmiPaymentDetailsDto
    {
        public int EmiScheduleId { get; set; }

        public int InstallmentNo { get; set; }

        public decimal EmiPay { get; set; }

        public decimal Bonus { get; set; }

        public decimal Penalty { get; set; }

        public int LateDays { get; set; }

        public decimal GST { get; set; }

        public decimal TotalAmount { get; set; }

    }
}
