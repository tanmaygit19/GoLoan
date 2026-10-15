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
        public int cibilReportId { get; set; }

        [ForeignKey("customers")]
        public int customerId { get; set; }
        public Customer customers { get; set; }

        public string panNo { get; set; }

        public int cibilScore { get; set; }

        public DateTime checkDate { get; set; }
    }
}

    
