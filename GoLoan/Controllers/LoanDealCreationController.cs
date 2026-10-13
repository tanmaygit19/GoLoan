using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanDealCreationController : ControllerBase
    {
            ILoanDealCreation service;


            public LoanDealCreationController(ILoanDealCreation service)
            {
                this.service = service;
            }


            [HttpPost]
            [Route("ApplyLoan")]
            public IActionResult addloan(ApplyLoanDTO app)
            {
                service.addloan(app);
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


