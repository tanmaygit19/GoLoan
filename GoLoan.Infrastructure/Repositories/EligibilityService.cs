using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;

namespace GoLoan.Infrastructure.Repositories
{
    public class EligibilityService : IEligibilityService
    {
        AppDbContext db;

        public EligibilityService(AppDbContext db)
        {
            this.db = db;
        }

        public EligibilityResult CheckEligibility(int id)
        {
            // Find customer
            var customer = db.Customers.Find(id);

            if (customer == null)
            {
                return null;
            }

            // Find latest CIBIL report
            var cibilReport = db.CibilReports
                .Where(x => x.customerId == id)
                .OrderByDescending(x => x.checkDate)
                .FirstOrDefault();

            if (cibilReport == null)
            {
                return null;
            }

            // Check eligibility based on CIBIL score
            string isEligible;
            string? rejectionReason = null;

            if (cibilReport.cibilScore >= 800)
            {
                isEligible = "Approved";
            }
            else if (cibilReport.cibilScore >= 650)
            {
                isEligible = "Manual Review";
            }
            else
            {
                isEligible = "Rejected";
                rejectionReason = "CIBIL score is below 650.";
            }

            // Get loan amount from Score Card
            var scoreCard = db.ScoreCards
                .Where(x => x.CustomerId == id)
                .OrderByDescending(x => x.ScoreCardId)
                .FirstOrDefault();

            decimal loanAmount = 0;

            if (scoreCard != null)
            {
                loanAmount = scoreCard.EligibleLoanAmount;
            }

            // Create eligibility result
            EligibilityResult result = new EligibilityResult();

            result.CustomerId = id;
            result.CibilScore = cibilReport.cibilScore;
            result.IsEligible = isEligible;
            result.LoanAmount = loanAmount;
            result.RejectionReason = rejectionReason;

            // Save result
            db.EligibilityResults.Add(result);
            db.SaveChanges();

            return result;
        }
    }
}
