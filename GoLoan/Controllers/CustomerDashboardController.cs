using GoLoan.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerDashboardController : Controller
    {
        private readonly ICustomerDashboardService service;

        public CustomerDashboardController(ICustomerDashboardService service)
        {
            this.service = service;
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
    }
}
    
 