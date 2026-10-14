using GoLoan.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreCardController : ControllerBase
    {
        IScoreCardService scoreCardService;

        public ScoreCardController(IScoreCardService service)
        {
            scoreCardService = service;
        }

        [HttpGet("{id}")]
        public IActionResult GenerateScoreCard(int id)
        {
            var scoreCard = scoreCardService.GenerateScoreCard(id);

            if (scoreCard == null)
            {
                return NotFound("Customer or CIBIL report not found.");
            }

            return Ok(new
            {
                scoreCard.ScoreCardId,
                scoreCard.CustomerId,
                scoreCard.RiskCategory,
                scoreCard.EligibleLoanAmount
            });
        }
    }
}