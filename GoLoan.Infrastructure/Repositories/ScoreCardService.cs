using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;

namespace GoLoan.Infrastructure.Repositories
{
    public class ScoreCardService : IScoreCardService
    {
        AppDbContext db;

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

            // Find CIBIL report
            var cibilReport = db.CibilReports
                .Where(x => x.customerId == id)
            var cibilReport = db.CibilReports.Where(x => x.customerId == id)
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

            // Calculate eligible loan amount
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

            // Check existing score card
            var scoreCard = db.ScoreCards
                .FirstOrDefault(x => x.CustomerId == id);

            if (scoreCard == null)
            {
                scoreCard = new ScoreCard();
                scoreCard.CustomerId = id;

                db.ScoreCards.Add(scoreCard);
            }

            // Update score card
            scoreCard.RiskCategory = riskCategory;
            scoreCard.EligibleLoanAmount = eligibleLoanAmount;

            // Save changes
            double factor =(Math.Pow(1 + rate, totalInstallments) - 1) /
                (rate * Math.Pow(1 + rate, totalInstallments));

            decimal eligibleLoanAmount = availableEMI * (decimal)factor;

            ScoreCard scoreCard = new ScoreCard();
            scoreCard.CustomerId = id;
            scoreCard.RiskCategory = riskCategory;
            scoreCard.EligibleLoanAmount = Math.Round(eligibleLoanAmount, 2);
            db.ScoreCards.Add(scoreCard);
            db.SaveChanges();
            return scoreCard;
        }

        public List<Customer> GetCustomers()
        {
            return db.Customers.ToList();
        }
    }
}