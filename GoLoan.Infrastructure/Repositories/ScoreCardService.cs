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

            var cibilReport = db.CibilReports.Where(x => x.customerId == id)
                .OrderByDescending(x => x.checkDate)
                .FirstOrDefault();

            if (cibilReport == null)
            {
                return null;
            }

            string riskCategory;

            if (cibilReport.cibilScore >= 900)
            {
                riskCategory = "Excellent";
            }
            else if (cibilReport.cibilScore >= 800)
            {
                riskCategory = "Very Good";
            }
            else if (cibilReport.cibilScore >= 750)
            {
                riskCategory = "Good";
            }
            else if (cibilReport.cibilScore >= 700)
            {
                riskCategory = "Average";
            }
            else if (cibilReport.cibilScore >= 650)
            {
                riskCategory = "Risky";
            }
            else
            {
                riskCategory = "Reject";
            }

            // Calculate eligible loan amount
            decimal monthlyIncome = customer.MonthlyIncome;
            decimal existingMonthlyEMI = 0;

            decimal availableEMI = monthlyIncome - existingMonthlyEMI;
                
            decimal annualInterestRate = 11;
            int tenureInYears = 5;

            decimal monthlyInterestRate = annualInterestRate / 1200;
            int totalInstallments = tenureInYears * 12;

            double rate = (double)monthlyInterestRate;

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
    }
}