using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;

namespace GoLoan.Infrastructure.Repositories
{
    public class EligibilityService : IEligibilityService
    {
        private readonly AppDbContext db;

        public EligibilityService(AppDbContext db)
        {
            this.db = db;
        }

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
                .OrderByDescending(x => x.checkDate)
                .FirstOrDefault();

            if (cibilReport == null)
            {
                return null;
            }

            var scoreCard = db.ScoreCards
                .Where(x => x.CustomerId == id)
                .OrderByDescending(x => x.ScoreCardId)
                .FirstOrDefault();

            if (scoreCard == null)
            {
                return null;
            }

            decimal eligibleAmount = scoreCard.EligibleLoanAmount;

            if (requestedLoanAmount <= 0 || requestedLoanAmount > eligibleAmount)
            {
                return null;
            }

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

            decimal monthlyRate = interestRate / 1200m;
            double rate = (double)monthlyRate;
            double factor = Math.Pow(1 + rate, loanTenureMonths);

            decimal emi = requestedLoanAmount *
                (decimal)(rate * factor / (factor - 1));

            emi = Math.Round(emi, 2);

            string status;
            string? rejectionReason = null;

            if (cibilReport.cibilScore >= 800)
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
            var result = new EligibilityResult
            {
                CustomerId = id,
                Customer = customer,
                CibilScore = cibilReport.cibilScore,
                IsEligible = status,
                LoanAmount = eligibleAmount,
                RequestedLoanAmount = requestedLoanAmount,
                LoanTenureMonths = loanTenureMonths,
                InterestRate = interestRate,
                RejectionReason = rejectionReason
            };

            db.EligibilityResults.Add(result);
            db.SaveChanges();
            return result;
        }

        public EligibilityResult? ApproveApplication(int eligibilityId)
        {
            var result = db.EligibilityResults.Find(eligibilityId);

            if (result == null || result.IsEligible != "Manual Review")
            {
                return null;
            }

            result.IsEligible = "Approved";
            result.RejectionReason = null;

            db.SaveChanges();

            return result;
        }

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

        public List<EligibilityResult> GetManualReviewApplications()
        {
            return db.EligibilityResults
                .Where(x => x.IsEligible == "Manual Review")
                .OrderBy(x => x.EligibilityId)
                .ToList();
        }
    }
}
