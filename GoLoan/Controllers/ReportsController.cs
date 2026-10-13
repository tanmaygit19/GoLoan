using GoLoan.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService service;

        public ReportsController(IReportsService service)
        {
            this.service = service;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetDashboard(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "From date cannot be later than To date."
                });
            }

            var result = await service.GetReportsDashboardAsync(fromDate, toDate);

            return Ok(new
            {
                Success = true,
                Message = "Reports dashboard fetched successfully.",
                Data = result
            });
        }
    }
}

