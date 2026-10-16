using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ForeClosureController : ControllerBase
    {
        IForeClosureService service;
        private readonly IEmiSchedular emiSchedular;

        public ForeClosureController(IForeClosureService service, IEmiSchedular emiSchedular)
        {
            this.service = service;
            this.emiSchedular = emiSchedular;
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

        [HttpGet]
        [Route("GetReq")]
        public async Task<IActionResult> GetReq()
        {
            try
            {
                var data = await service.GetAllForeClosureRequests();
                return Ok(data);
            }
            catch(Exception ex)
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
                return Ok(new {message= "Foreclosure payment successful" });
            }

            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetReqForAdmin")]
        public async Task<IActionResult> GetReqForAdmin()
        {
            try
            {
                var data = await service.GetAllForeClosureRequestsForAdmin();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("GetAllClosureRequests")]
        public async Task<IActionResult> GetAllClosureRequests()
        {
            var result = await service.GetAllClosureRequests();
            return Ok(result);
        }


    }
}
