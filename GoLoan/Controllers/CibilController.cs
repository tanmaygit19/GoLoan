using GoLoan.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CibilController : ControllerBase
    {
        ICibilService service;

        public CibilController(ICibilService service)
        {
            this.service = service;
        }

        // Generate CIBIL report
        [HttpPost("{id}")]
        public IActionResult GenerateCibilReport(int id)
        {
            var report = service.GenerateCibilReport(id);

            if (report == null)
            {
                return NotFound("Customer not found.");
            }

            return Ok(report);
        }

        // View latest saved CIBIL report
        [HttpGet("{id}")]
        public IActionResult GetCibilReport(int id)
        {
            var report = service.GetCibilReport(id);

            if (report == null)
            {
                return NotFound("CIBIL report not found.");
            }

            return Ok(report);
        }
    }
}
