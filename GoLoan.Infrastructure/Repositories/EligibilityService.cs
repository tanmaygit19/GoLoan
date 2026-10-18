
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

        // Get latest eligibility details
        public EligibilityResult? CheckEligibility(int id)
        {
            var result = db.EligibilityResults
                .Where(x => x.CustomerId == id)
                .OrderByDescending(x => x.EligibilityId)
                .FirstOrDefault();

            if (result != null)
            {
                result.Customer = db.Customers.Find(result.CustomerId);
            }

            return result;
        }

        // Customer applies for a loan
        public EligibilityResult? ApplyForLoan(
            int id,
            decimal requestedLoanAmount,
            int loanTenureMonths)
        {
            var customer = db.Customers.Find(id);
            if (customer == null)
            {
                return null;
            }

            var cibilReport = db.CibilReports
                .Where(x => x.customerId == id)
            var cibilReport = db.CibilReports.Where(x => x.customerId == id)
                .OrderByDescending(x => x.checkDate)
                .FirstOrDefault();

            if (cibilReport == null)
            {
                return null;
            }

            // Get maximum eligible amount from Score Card
            var scoreCard = db.ScoreCards
                .Where(x => x.CustomerId == id)
                .OrderByDescending(x => x.ScoreCardId)
                .FirstOrDefault();
            string isEligible;
            string? rejectionReason = null;

            if (scoreCard == null)
            {
                return null;
            }

            decimal eligibleAmount = scoreCard.EligibleLoanAmount;

            // Validate requested amount
            if (requestedLoanAmount <= 0 ||
                requestedLoanAmount > eligibleAmount)
            {
                return null;
            }

            // Select interest rate based on tenure
            decimal interestRate;

            if (loanTenureMonths == 36)
            {
                interestRate = 11.5m;
            }
            else if (loanTenureMonths == 60 ||
                     loanTenureMonths == 72 ||
                     loanTenureMonths == 120)
            {
                interestRate = 11m;
            }
            else
            {
                return null;
            }

            // Calculate monthly EMI
            decimal monthlyRate = interestRate / 1200m;
            double rate = (double)monthlyRate;
            double factor = Math.Pow(1 + rate, loanTenureMonths);

            decimal emi = requestedLoanAmount *
                (decimal)(rate * factor / (factor - 1));

            emi = Math.Round(emi, 2);

            // Decide eligibility status
            string status;
            string? rejectionReason = null;

            if (cibilReport.cibilScore >= 800)
            var scoreCard = db.ScoreCards.Where(x => x.CustomerId == id)
                .OrderByDescending(x => x.ScoreCardId)
                .FirstOrDefault();

            decimal loanAmount = 0;
            if (scoreCard != null)
            {
                status = "Auto Approved";
            }
            else if (cibilReport.cibilScore >= 650)
            {
                status = "Manual Review";
            }
            else
            {
                status = "Auto Rejected";
                rejectionReason = "CIBIL score below 650.";
            }

            // Save eligibility result
            var result = new EligibilityResult();
            EligibilityResult result = new EligibilityResult();

            result.CustomerId = id;
            result.Customer = customer;
            result.CibilScore = cibilReport.cibilScore;
            result.IsEligible = status;
            result.LoanAmount = eligibleAmount;
            result.RequestedLoanAmount = requestedLoanAmount;
            result.LoanTenureMonths = loanTenureMonths;
            result.InterestRate = interestRate;
            result.RejectionReason = rejectionReason;

            db.EligibilityResults.Add(result);
            db.SaveChanges();
            return result;
        }

        // Officer approves application
        public EligibilityResult? ApproveApplication(int eligibilityId)
        {
            var result = db.EligibilityResults.Find(eligibilityId);

            if (result == null ||
                result.IsEligible != "Manual Review")
            {
                return null;
            }

            result.IsEligible = "Approved";
            result.RejectionReason = null;

            db.SaveChanges();

            return result;
        }

        // Officer rejects application
        public EligibilityResult? RejectApplication(
            int eligibilityId,
            string rejectionReason)
        {
            var result = db.EligibilityResults.Find(eligibilityId);

            if (result == null ||
                result.IsEligible != "Manual Review" ||
                string.IsNullOrWhiteSpace(rejectionReason))
            {
                return null;
            }

            result.IsEligible = "Rejected";
            result.RejectionReason = rejectionReason.Trim();

            db.SaveChanges();

            return result;
        }

        // Get applications waiting for officer review
        public List<EligibilityResult> GetManualReviewApplications()
        {
            return db.EligibilityResults
                .Where(x => x.IsEligible == "Manual Review")
                .OrderBy(x => x.EligibilityId)
                .ToList();
        }
    }
}
