using GoLoan.Application.DTO.SupportTicket;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
    public interface ISupportTicketService
    {
        Task AddTicketAsync(AddSupportTicketDTO dto, int customerId);
        Task<List<SupportTicketDTO>> GetMyTicketsAsync(int customerId);
        Task<List<SupportTicketListDTO>> GetAllTicketsAsync();
        Task<SupportTicketDetailsDTO?> GetTicketByIdAsync(int ticketId);
        Task UpdateTicketAsync(int ticketId, UpdateSupportTicketDTO dto);
    }
}
