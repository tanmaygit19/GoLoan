using GoLoan.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerPaymentHistoryController : ControllerBase
    {
        ICustomerPaymentHistoryService service;

        public CustomerPaymentHistoryController(ICustomerPaymentHistoryService service)
        {
            this.service = service;
        }
        [HttpGet]
        [Route("EmiPayment")]
        public async Task<IActionResult> GetEmiPaymentHistory(int? loanAccount,string period = "all")
        {
            var result = await service.GetEmiPaymentHistory(loanAccount, period);

            return Ok(result);
        }

        [HttpGet]
        [Route("ForeClosurePayment")]
        public async Task<IActionResult> GetForeClosurePaymentHistory(int? loanAccount, string period = "all")
        {
            var result = await service.GetForeClosurePaymentHistory(loanAccount, period);

            return Ok(result);
        }
    }
}
