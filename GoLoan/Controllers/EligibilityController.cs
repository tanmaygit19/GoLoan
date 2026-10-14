using GoLoan.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EligibilityController : ControllerBase
    {
        IEligibilityService eligibilityService;

        public EligibilityController(IEligibilityService service)
        {
            this.eligibilityService = service;
        }

        [HttpGet("{id}")]
        public IActionResult CheckEligibility(int id)
        {
            var result = eligibilityService.CheckEligibility(id);

            if (result == null)
            {
                return NotFound("Customer or CIBIL report not found.");
            }

            return Ok(new
            {
                result.EligibilityId,
                result.CustomerId,
                result.CibilScore,
                result.IsEligible,
                result.LoanAmount,
                result.RejectionReason
            });
        }
    }
}
