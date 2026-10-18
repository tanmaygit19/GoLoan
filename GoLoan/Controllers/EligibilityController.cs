
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

        // Get customer eligibility
        [HttpGet("{id}")]
        public IActionResult CheckEligibility(int id)
        {
            var result = eligibilityService.CheckEligibility(id);

            if (result == null)
            {
                return NotFound("Eligibility result not found.");
            }

            var customer = result.Customer;

            if (customer == null)
            {
                return NotFound("Customer details not found.");
            }

            return Ok(new
            {
                result.EligibilityId,
                result.CustomerId,
                CustomerName = customer.FirstName + " " + customer.LastName,
                PanNumber = customer.Pan,
                customer.MonthlyIncome,
                result.CibilScore,
                result.IsEligible,
                result.LoanAmount,
                result.RequestedLoanAmount,
                result.LoanTenureMonths,
                result.InterestRate,
                result.RejectionReason
            });
        }

        // Customer applies for a loan
        [HttpPost("Apply/{id}")]
        public IActionResult ApplyForLoan(
            int id,
            [FromQuery] decimal requestedLoanAmount,
            [FromQuery] int loanTenureMonths)
        {
            var result = eligibilityService.ApplyForLoan(
                id,
                requestedLoanAmount,
                loanTenureMonths);

            if (result == null)
            {
                return BadRequest(
                    "Invalid customer, CIBIL report, loan amount, or tenure.");
            }

            var customer = result.Customer;

            if (customer == null)
            {
                return NotFound("Customer details not found.");
            }

            return Ok(new
            {
                result.EligibilityId,
                result.CustomerId,
                CustomerName = customer.FirstName + " " + customer.LastName,
                PanNumber = customer.Pan,
                customer.MonthlyIncome,
                result.CibilScore,
                result.IsEligible,
                result.LoanAmount,
                result.RequestedLoanAmount,
                result.LoanTenureMonths,
                result.InterestRate,
                result.RejectionReason
            });
        }

        // Officer approves application
        [HttpPut("Approve/{eligibilityId}")]
        public IActionResult ApproveApplication(int eligibilityId)
        {
            var result = eligibilityService.ApproveApplication(eligibilityId);

            if (result == null)
            {
                return BadRequest(
                    "Application not found or not awaiting manual review.");
            }

            return Ok(result);
        }

        // Officer rejects application
        [HttpPut("Reject/{eligibilityId}")]
        public IActionResult RejectApplication(
            int eligibilityId,
            [FromQuery] string rejectionReason)
        {
            var result = eligibilityService.RejectApplication(
                eligibilityId,
                rejectionReason);

            if (result == null)
            {
                return BadRequest(
                    "Application not found, not awaiting review, or rejection reason is empty.");
            }

            return Ok(result);
        }

        // Get applications waiting for officer review
        [HttpGet("ManualReview")]
        public IActionResult GetManualReviewApplications()
        {
            var result = eligibilityService.GetManualReviewApplications();

            return Ok(result.Select(x => new
            {
                x.EligibilityId,
                x.CustomerId,
                x.CibilScore,
                x.IsEligible,
                x.LoanAmount,
                x.RequestedLoanAmount,
                x.LoanTenureMonths,
                x.InterestRate,
                x.RejectionReason
            }).ToList());
        }
    }
}
