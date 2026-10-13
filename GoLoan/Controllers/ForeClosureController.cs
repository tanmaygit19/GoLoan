using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ForeClosureController : ControllerBase
    {
        IForeClosureService service;

        public ForeClosureController(IForeClosureService service)
        {
            this.service = service;

        }


        [HttpPost]
        [Route("AddR")]
        public async Task<IActionResult> AddRequest(ForeClosureRequestDto dto)
        {
            try
            {
                await service.CreateForeClosureRequest(dto);
                return Ok("Foreclosure Request Added Successfully");
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [Route("Approve/{requestId}")]
        public async Task<IActionResult> Approve(int requestId)
        {
            try {
                await service.ApproveRequest(requestId); 
                return Ok("Foreclosure Request Approved Successfully"); 
            }
            catch (Exception ex) {
                return BadRequest(ex.Message); 
            }

        }

        [HttpPut]
        [Route("Reject/{requestId}")]
        public async Task<IActionResult> Reject(int requestId)
        {
            try
            {
                await service.RejectRequest(requestId);

                return Ok("Foreclosure Request Rejected Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        [HttpPost]
        [Route("CreatePaymentOrder/{requestId}")]
        public async Task<IActionResult> CreatePaymentOrder(int requestId)
        {
            try
            {
                var orderId = await service.CreateForeClosurePaymentOrder(requestId);

                return Ok(new
                {
                    OrderId = orderId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Payment")]
        public async Task<IActionResult> VerifyPayment(VerifyForeClosurePaymentDto dto)
        {
            try
            {
                await service.VerifyForeClosurePayment(dto);
                return Ok("Foreclosure payment successful");
            }

            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
