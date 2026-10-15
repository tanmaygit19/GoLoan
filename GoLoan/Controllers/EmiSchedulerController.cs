using GoLoan.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;


namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmiSchedulerController : ControllerBase
    {
        private readonly AppDbContext db;

        public EmiSchedulerController(AppDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("GetEmiSchedule/{loanAccountId}")]
        public async Task<IActionResult> GetEmiSchedule(int loanAccountId)
        {

            var emis = await db.EmiSchedules
                .Where(e => e.LoanAccountId == loanAccountId)
                .OrderBy(e => e.InstallmentNo)
                .Select(e => new
                {
                    e.EmiScheduleId,
                    e.LoanAccountId,
                    e.InstallmentNo,
                    e.DueDate,
                    e.Emi,
                    e.PrincipalAmount,
                    e.InterestAmount,
                    e.OpeningBalance,
                    e.ClosingBalance,
                    e.PaymentStatus,
                    e.PaidDate
                })
                .ToListAsync();

            return Ok(emis);


           
        }
    }
}
