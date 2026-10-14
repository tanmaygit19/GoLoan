using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO.SupportTicket
{
    public class SupportTicketListDTO
    {
        public int TicketId { get; set; }
        public string LoanAccountNo { get; set; }
        public string CustomerName { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
