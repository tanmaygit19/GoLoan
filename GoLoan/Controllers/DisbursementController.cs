using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    public class DisbursementController : Controller
    {
        IDisbursement service;

        public DisbursementController(IDisbursement disbursement)
        {
            this.service = disbursement;
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
                message = "Disbursement Added Successfully"
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


        public IActionResult Index()
        {
            return View();
        }
    }
}
