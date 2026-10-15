using global::GoLoan.Domain.Entities;
using global::GoLoan.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace GoLoan.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("OfficerDashboard")]
        public async Task<IActionResult> GetOfficerDashboard()
        {
            try
            {
                var dashboard = new Dashboard
                {
                    TotalCustomers = await _db.Customers.CountAsync(),

                    TotalLoanCustomers = await _db.LoanDeals
                        .Select(x => x.CustomerId)
                        .Distinct()
                        .CountAsync(),

                    TotalApprovedKyc = await _db.KycDocuments
                        .Where(x => x.VerificationStatus == "Approved")
                        .Select(x => x.CustomerId)
                        .Distinct()
                        .CountAsync(),

                    TotalLoanAmount = await _db.LoanDeals
                        .SumAsync(x => (decimal?)x.LoanAmount) ?? 0,

                    TotalActiveLoans = await _db.LoanAccounts
                        .CountAsync(x => x.LoanStatus == "Active"),

                    TodaysCollection = await _db.LoanPayments
                        .Where(x => x.PaymentDate.Date == DateTime.Today && x.PaymentStatus == "Success")
                        .SumAsync(x => (decimal?)x.PaidAmount) ?? 0,

                    PendingEmiCount = await _db.EmiSchedules
                        .CountAsync(x => x.PaymentStatus == "Pending"),

                    ForeClosureRequests = await _db.ForeClosureRequests
                        .CountAsync(x => x.Status == "Pending")
                };

                return Ok(new
                {
                    success = true,
                    data = dashboard
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
