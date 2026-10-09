using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO.SupportTicket
{
    public class SupportTicketDetailsDTO
    {
        public int TicketId { get; set; }
        public string CustomerName { get; set; }
        public string LoanAccountNo { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string? AttachmentPath { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string? OfficerResponse { get; set; }
    }
}
