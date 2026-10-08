using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Domain.Entities
{
    public class CibilReport
    {
        [Key]
        public int CibilReportId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public string PanNo { get; set; }

        public int CibilScore { get; set; }

        public DateTime CheckDate { get; set; }

        public Customer? Customer { get; set; }
    }
}