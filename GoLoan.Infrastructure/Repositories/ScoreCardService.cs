using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;

namespace GoLoan.Infrastructure.Repositories
{
    public class ScoreCardService : IScoreCardService
    {
        private readonly AppDbContext db;

        public ScoreCardService(AppDbContext db)
        {
            this.db = db;
        }

        public ScoreCard? GenerateScoreCard(int id)
        {
            var customer = db.Customers.Find(id);

            if (customer == null)
            {
                return null;
            }

            var cibilReport = db.CibilReports
                .Where(x => x.customerId == id)
                .OrderByDescending(x => x.checkDate)
                .FirstOrDefault();

            if (cibilReport == null)
            {
                return null;
            }

            string riskCategory;

            if (cibilReport.cibilScore >= 750)
            {
                riskCategory = "Low";
            }
            else if (cibilReport.cibilScore >= 650)
            {
                riskCategory = "Medium";
            }
            else
            {
                riskCategory = "High";
            }

            decimal monthlyIncome = customer.MonthlyIncome;
            decimal eligibleLoanAmount;

            if (monthlyIncome > 100000)
            {
                eligibleLoanAmount = 5000000;
            }
            else if (monthlyIncome > 50000)
            {
                eligibleLoanAmount = 4000000;
            }
            else
            {
                eligibleLoanAmount = 1000000;
            }

            var scoreCard = db.ScoreCards
                .FirstOrDefault(x => x.CustomerId == id);

            if (scoreCard == null)
            {
                scoreCard = new ScoreCard
                {
                    CustomerId = id
                };

                db.ScoreCards.Add(scoreCard);
            }

            scoreCard.RiskCategory = riskCategory;
            scoreCard.EligibleLoanAmount = eligibleLoanAmount;

            db.SaveChanges();

            return scoreCard;
        }

        public List<Customer> GetCustomers()
        {
            return db.Customers.ToList();
        }
    }
}
