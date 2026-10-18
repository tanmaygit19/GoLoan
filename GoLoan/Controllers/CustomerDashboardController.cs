
using GoLoan.Application.Interfaces;
using GoLoan.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoLoan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerDashboardController : Controller
    {
        private readonly ICustomerDashboardService service;
        private readonly AppDbContext db;

        public CustomerDashboardController(
            ICustomerDashboardService service,
            AppDbContext db)
        {
            this.service = service;
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard(int customerId)
        {
            var dashboard = await service.GetDashboardAsync(customerId);

            return Ok(new
            {
                Success = true,
                Message = "Dashboard fetched successfully.",
                Data = dashboard
            });
        }

        [HttpGet("CustomerDetails/{customerId}")]
        public async Task<IActionResult> GetCustomerDetails(int customerId)
        {
            var customer = await db.Customers
                .Where(x => x.CustomerId == customerId)
                .Select(x => new
                {
                    x.CustomerId,
                    x.FirstName,
                    x.LastName,
                    x.Pan
                })
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
    }
}
