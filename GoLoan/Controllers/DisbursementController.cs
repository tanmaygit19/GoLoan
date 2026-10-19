using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisbursementController : ControllerBase
    {
        private readonly IDisbursement service;
        private readonly ILoanAccountService loanAccountService;
        private readonly IBackgroundJobClient backgroundJobClient;

        public DisbursementController(
            IDisbursement disbursement,
            ILoanAccountService loanAccountService,
            IBackgroundJobClient backgroundJobClient)
        {
            service = disbursement;
            this.loanAccountService = loanAccountService;
            this.backgroundJobClient = backgroundJobClient;
        }

        [HttpPost]
        [Route("AddDisbursement")]
        public IActionResult adddisbursement(DisbursementDTO dis)
        {
            service.adddisbursement(dis);

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = null,
                message = "Disbursement Added Successfully"
            });
        }

        [HttpGet]
        [Route("FetchDisbursement")]
        public IActionResult fetchdisbursement()
        {
            var data = service.fetchdisbursement();

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = data,
                message = "Disbursements Fetched Successfully"
            });
        }

        [HttpGet]
        [Route("FetchDisbursementById")]
        public IActionResult fetchdisbursementbyid(int id)
        {
            var data = service.fetchdisbursementbyid(id);

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = data,
                message = "Disbursement Fetched Successfully"
            });
        }

        [HttpPut]
        [Route("ApproveDisbursement")]
        public async Task<IActionResult> approvedisbursement(
            int disbursementid)
        {
            var disbursement =
                service.fetchdisbursementbyid(disbursementid);

            if (disbursement == null)
            {
                return NotFound(new
                {
                    message = "Disbursement not found"
                });
            }

            service.approvedisbursement(disbursementid);

            disbursement =
                service.fetchdisbursementbyid(disbursementid);

            if (disbursement == null ||
                disbursement.Status != "Approved")
            {
                return BadRequest(new
                {
                    message = "Disbursement is not approved"
                });
            }

            bool accountExists =
                await loanAccountService.AccountExist(
                    disbursement.DealId);

            if (!accountExists)
            {
                var account =
                    await loanAccountService.CreateLoanAccount(
                        disbursement.DealId);

                if (account == null)
                {
                    return BadRequest(new
                    {
                        message =
                            "Disbursement approved, but Loan Account could not be created."
                    });
                }

                backgroundJobClient.Enqueue<IEmiSchedular>(
                    x => x.GenerateEmiSchedule(
                        account.LoanAccountId));
            }

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = null,
                message =
                    "Disbursement Approved. Loan Account checked/created successfully."
            });
        }

        [HttpPut]
        [Route("RejectDisbursement")]
        public IActionResult rejectdisbursement(int disbursementid)
        {
            service.rejectdisbursement(disbursementid);

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = null,
                message = "Disbursement Rejected Successfully"
            });
        }

        [HttpGet]
        [Route("FetchDisbursementsByStatus")]
        public IActionResult fetchdisbursementsbystatus(string status)
        {
            var data = service.fetchdisbursementsbystatus(status);

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = data,
                message = "Disbursements Fetched Successfully"
            });
        }
    }
}