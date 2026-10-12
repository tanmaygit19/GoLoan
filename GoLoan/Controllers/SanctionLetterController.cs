
using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    public class SanctionLetterController : Controller
    {
        Isanctionletter service;

        public SanctionLetterController(Isanctionletter service)
        {
            this.service = service;
        }

        [HttpPost]
        [Route("GenerateSanction")]
        public IActionResult generatesanction(int loandealid)
        {
            var id = service.generatesanction(loandealid);

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = null,
                message = "Sanction Generated Successfully"
            });
        }

        [HttpGet]
        [Route("FetchSanction")]
        public IActionResult fetchsanction()
        {
            var data = service.fetchsanction();

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = data,
                message = "Sanction Details Fetched Successfully"
            });
        }

        [HttpGet]
        [Route("FetchSanctionById")]
        public IActionResult fetchsanctionbyid(int sanctionid)
        {
            var data = service.fetchsanctionbyid(sanctionid);

            return Ok(new CommonResponse
            {
                success = true,
                error = false,
                data = data,
                message = "Sanction Details Fetched Successfully"
            });
        }
    }
}