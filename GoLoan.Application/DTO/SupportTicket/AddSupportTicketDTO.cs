using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.DTO.SupportTicket
{
    public class AddSupportTicketDTO
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        public IFormFile? Attachment { get; set; }
    }
}
