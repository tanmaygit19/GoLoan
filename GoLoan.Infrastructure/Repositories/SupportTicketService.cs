using GoLoan.Application.DTO.SupportTicket;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GoLoan.Infrastructure.Repositories
{
    public class SupportTicketService : ISupportTicketService
    {
        private readonly AppDbContext db;

        public SupportTicketService(AppDbContext db)
        {
            this.db = db;
        }
        public async Task AddTicketAsync(AddSupportTicketDTO dto, int customerId)
        {
            var loanAccount = db.LoanAccounts.FirstOrDefault(x => x.CustomerId == customerId && x.LoanStatus == "Active");

            if (loanAccount == null)
            {
                throw new Exception("No active loan account found for this customer.");
            }

            string? attachmentPath = null;

            if (dto.Attachment != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Attachment.FileName);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Attachment.CopyToAsync(stream);
                }

                attachmentPath = "/uploads/" + fileName;
            }

            var ticket = new SupportTicket
            {
                CustomerId = customerId,
                LoanAccountId = loanAccount.LoanAccountId,
                Subject = dto.Subject,
                Description = dto.Description,
                AttachmentPath = attachmentPath,
                CreatedDate = DateTime.Now,
                Status = "Pending",
                OfficerResponse = null
            };

            db.SupportTickets.Add(ticket);
            await db.SaveChangesAsync();
        }

        public async Task<List<SupportTicketListDTO>> GetAllTicketsAsync()
        {
            var tickets = await db.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.LoanAccount)
                .Select(x => new SupportTicketListDTO
                {
                    TicketId = x.TicketId,
                    LoanAccountNo = x.LoanAccount.LoanAccountNo,
                    CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
                    Subject = x.Subject,
                    Status = x.Status,
                    CreatedDate = x.CreatedDate
                })
                .ToListAsync();

            return tickets;
        }

        public async Task<List<SupportTicketDTO>> GetMyTicketsAsync(int customerId)
        {
            var tickets = await db.SupportTickets
                .Where(x => x.CustomerId == customerId)
                .Select(x => new SupportTicketDTO
                {
                    TicketId = x.TicketId,
                    Subject = x.Subject,
                    Description = x.Description,
                    AttachmentPath = x.AttachmentPath,
                    CreatedDate = x.CreatedDate,
                    Status = x.Status,
                    OfficerResponse = x.OfficerResponse
                })
                .ToListAsync();

            return tickets;
        }

        public async Task<SupportTicketDetailsDTO?> GetTicketByIdAsync(int ticketId)
        {
            var ticket = await db.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.LoanAccount)
                .Where(x => x.TicketId == ticketId)
                .Select(x => new SupportTicketDetailsDTO
                {
                    TicketId = x.TicketId,
                    CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
                    LoanAccountNo = x.LoanAccount.LoanAccountNo,
                    Subject = x.Subject,
                    Description = x.Description,
                    AttachmentPath = x.AttachmentPath,
                    CreatedDate = x.CreatedDate,
                    Status = x.Status,
                    OfficerResponse = x.OfficerResponse
                })
                .FirstOrDefaultAsync();

            return ticket;
        }

        public async Task UpdateTicketAsync(int ticketId, UpdateSupportTicketDTO dto)
        {
            var ticket = await db.SupportTickets
                .FirstOrDefaultAsync(x => x.TicketId == ticketId);

            if (ticket == null)
            {
                throw new Exception("Support ticket not found.");
            }

            ticket.Status = dto.Status;
            ticket.OfficerResponse = dto.OfficerResponse;

            await db.SaveChangesAsync();
        }
    }
}
