using GoLoan.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LMSReportsController : ControllerBase
    {
        private readonly ILMSReportsService service;

        public LMSReportsController(ILMSReportsService service)
        {
            this.service = service;
        }

        // 1. LMS Dashboard
        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "From date cannot be greater than To date."
                });
            }

            var data = await service.GetDashboardAsync(fromDate, toDate);

            return Ok(new
            {
                Success = true,
                Message = "LMS reports dashboard fetched successfully.",
                Data = data
            });
        }

        // 2. Loan Portfolio Report
        [HttpGet("LoanPortfolio")]
        public async Task<IActionResult> GetLoanPortfolio()
        {
            var data = await service.GetLoanPortfolioReportAsync();

            return Ok(new
            {
                Success = true,
                Message = "Loan portfolio report fetched successfully.",
                Data = data
            });
        }

        // 3. EMI Collection Report
        [HttpGet("EMICollection")]
        public async Task<IActionResult> GetEMICollection([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "From date cannot be greater than To date."
                });
            }

            var data = await service.GetEMICollectionReportAsync(fromDate, toDate);

            return Ok(new
            {
                Success = true,
                Message = "EMI collection report fetched successfully.",
                Data = data
            });
        }

        // 4. Overdue Report
        [HttpGet("Overdue")]
        public async Task<IActionResult> GetOverdue([FromQuery] DateTime? asOnDate)
        {
            var data = await service.GetOverdueReportAsync(asOnDate);

            return Ok(new
            {
                Success = true,
                Message = "Overdue report fetched successfully.",
                Data = data
            });
        }

        // 5. Disbursement Report
        [HttpGet("Disbursement")]
        public async Task<IActionResult> GetDisbursement([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "From date cannot be greater than To date."
                });
            }

            var data = await service.GetDisbursementReportAsync(fromDate, toDate);

            return Ok(new
            {
                Success = true,
                Message = "Disbursement report fetched successfully.",
                Data = data
            });
        }

        // 6. Prepayment Report
        [HttpGet("Prepayment")]
        public async Task<IActionResult> GetPrepayment([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "From date cannot be greater than To date."
                });
            }

            var data = await service.GetPrepaymentReportAsync(fromDate, toDate);

            return Ok(new
            {
                Success = true,
                Message = "Prepayment report fetched successfully.",
                Data = data
            });
        }
    }
}