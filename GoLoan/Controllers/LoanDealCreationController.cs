using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.Design;

namespace GoLoan.Controllers
{
    public class LoanDealCreationController : Controller
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
    }
}
