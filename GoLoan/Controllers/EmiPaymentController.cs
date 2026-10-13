using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmiPaymentController : ControllerBase
    {
        IEmiPaymentService service;
        public EmiPaymentController(IEmiPaymentService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("Details/{emiScheduleId}")]
        public async Task<IActionResult> GetData(int emiScheduleId)
        {
            try
            {
                var data = await service.GetEmiPaymentDetails(emiScheduleId);
                return Ok(data);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateOrder/{emiScheduleId}")]
        public async Task<IActionResult> createOrderId(int emiScheduleId)
        {
            try
            {
                var orderId = await service.CreateEmiPaymentOrder(emiScheduleId);

                return Ok(new
                {
                    EmiScheduleId = emiScheduleId,
                    orderId = orderId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

            [HttpPost]
            [Route("Verify")]
            public async Task<IActionResult> verify(VerifyEmiPaymentDto dto)
            {
            try
            {
                await service.VerifyEmiPayment(dto);

                return Ok(new { Message = "EMI payment verified successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
    }
}
