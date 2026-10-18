using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanDealCreationController : ControllerBase
    {
        ILoanDealCreation service;
        AppDbContext db;

        public LoanDealCreationController(ILoanDealCreation service, AppDbContext db)
        {
            this.service = service;
            this.db = db;
        }

        [HttpPost]
        [Route("ApplyLoan")]
        public async Task<IActionResult> addloan(ApplyLoanDTO app)
        {
            int customerid = 1;

            if (app.LoanAmount <= 0)
            {
                return BadRequest(new CommonResponse
                {
                    success = false,
                    error = true,
                    data = null,
                    message = "Loan amount must be greater than zero."
                });
            }

            var eligibility = await db.EligibilityResults
                .Where(x => x.CustomerId == customerid)
                .OrderByDescending(x => x.EligibilityId)
                .FirstOrDefaultAsync();

            if (eligibility == null)
            {
                return BadRequest(new CommonResponse
                {
                    success = false,
                    error = true,
                    data = null,
                    message = "Please check your loan eligibility first."
                });
            }

            if (eligibility.IsEligible != "Approved")
            {
                return BadRequest(new CommonResponse
                {
                    success = false,
                    error = true,
                    data = null,
                    message = "You are not eligible to apply for this loan."
                });
            }

            if (app.LoanAmount > eligibility.LoanAmount)
            {
                return BadRequest(new CommonResponse
                {
                    success = false,
                    error = true,
                    data = null,
                    message = "You are eligible for a loan up to ₹" +
                              eligibility.LoanAmount + " only."
                });
            }

            service.addloan(app, customerid);

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = null,
                message = "Loan Applied Success"
            });
        }

        [HttpGet]
        [Route("FetchLoanDetails")]
        public IActionResult fetchdetails()
        {
            var data = service.fetchdetails();

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = data,
                message = "Loan Details Fetched Successfully"
            });
        }

        [HttpGet]
        [Route("FetchLoanById")]
        public IActionResult fetchloanbycustomerid(int customerid)
        {
            var data = service.fetchloanbyid(customerid);

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = data,
                message = "Loan Details Fetched Successfully"
            });
        }

        [HttpPut]
        [Route("ApproveLoan")]
        public IActionResult approveloan(LoanApprovalDTO app)
        {
            service.approveloan(app);

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = null,
                message = "Loan Approved Successfully"
            });
        }

        [HttpPut]
        [Route("RejectLoan")]
        public IActionResult rejectloan(LoanApprovalDTO app)
        {
            service.rejectloan(app);

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = null,
                message = "Loan Rejected Successfully"
            });
        }

        [HttpGet]
        [Route("FetchLoansByStatus")]
        public IActionResult fetchloansbystatus(string status)
        {
            var data = service.fetchloansbystatus(status);

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = data,
                message = "Loans Fetched Successfully"
            });
        }



    }
}