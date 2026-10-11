using GoLoan.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService service;
        public NotificationController(INotificationService service)
        {
            this.service = service;
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetNotifications(int customerId)
        {
            var notifications = await service.GetNotifications(customerId);
            return Ok(notifications);
        }

      
        [HttpPut("Read/{notificationId}/{customerId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId, int customerId)
        {
            await service.MarkAsRead(notificationId, customerId);
            return Ok("Notification marked as read.");
        }
        [HttpPost("TestEmiReminders")]
        public async Task<IActionResult> TestEmiReminders()
        {
            await service.CreateEmiReminders();
            return Ok("EMI reminders checked and created if due tomorrow.");
        }
    }

}
