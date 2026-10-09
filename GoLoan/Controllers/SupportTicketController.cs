using GoLoan.Application.DTO.SupportTicket;
using GoLoan.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportTicketController : Controller
    {
        private readonly ISupportTicketService service;

        public SupportTicketController(ISupportTicketService service)
        {
            this.service = service;
        }
        [HttpPost]
        [Route("AddTicket")]
        public async Task<IActionResult> AddTicket([FromForm] AddSupportTicketDTO dto, int customerId)
        {
            await service.AddTicketAsync(dto, customerId);

            return Ok(new
            {
                Success = true,
                Message = "Support ticket raised successfully."
            });
        }

        [HttpGet]
        [Route("MyTickets")]
        public async Task<IActionResult> MyTickets(int customerId)
        {
            var tickets = await service.GetMyTicketsAsync(customerId);

            return Ok(new
            {
                Success = true,
                Message = "Tickets fetched successfully.",
                Data = tickets
            });
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await service.GetAllTicketsAsync();

            return Ok(new
            {
                Success = true,
                Message = "All support tickets fetched successfully.",
                Data = tickets
            });
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await service.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Support ticket not found."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Support ticket fetched successfully.",
                Data = ticket
            });
        }

        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, UpdateSupportTicketDTO dto)
        {
            await service.UpdateTicketAsync(id, dto);

            return Ok(new
            {
                Success = true,
                Message = "Support ticket updated successfully."
            });
        }
    }
}


