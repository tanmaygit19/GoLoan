using GoLoan.Application.DTO;
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisbursementController : ControllerBase
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

            [HttpPut]
            [Route("ApproveDisbursement")]
            public IActionResult approvedisbursement(int disbursementid)
            {
                service.approvedisbursement(disbursementid);

                return Ok(new CommonResponse
                {
                    success = true,
                    error = false,
                    data = null,
                    message = "Disbursement Approved Successfully"
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


