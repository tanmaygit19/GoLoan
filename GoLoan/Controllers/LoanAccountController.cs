using GoLoan.Application.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanAccountController : ControllerBase
    {
        private readonly ILoanAccountService service;
        private readonly IBackgroundJobClient bj;

        public LoanAccountController(ILoanAccountService service, IBackgroundJobClient bj)
        {
            this.service = service;
            this.bj = bj;
        }


        [HttpPost]
        [Route("Create/{dealId}")]
        public async Task<IActionResult> CreateLoanAccount(int dealId)
        {
            var account = await service.CreateLoanAccount(dealId);

            if (account == null)
            {
                return BadRequest("Loan account could not be created. Check the deal, disbursement status, or existing account.");
            }
            bj.Enqueue<IEmiSchedular>(x => x.GenerateEmiSchedule(account.LoanAccountId));
            return Ok(account);
        }


        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await service.GetAccounts();
            return Ok(accounts);
        }


        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = await service.GetById(id);

            if (account == null)
            {
                return NotFound("Loan account not found.");
            }

            return Ok(account);
        }

    }
}