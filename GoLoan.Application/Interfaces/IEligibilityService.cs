
using GoLoan.Domain.Entities;

namespace GoLoan.Application.Interfaces
{
    public interface IEligibilityService
    {
        EligibilityResult? CheckEligibility(int id);

        EligibilityResult? ApplyForLoan(
            int id,
            decimal requestedLoanAmount,
            int loanTenureMonths);

        EligibilityResult? ApproveApplication(int eligibilityId);

        EligibilityResult? RejectApplication(
            int eligibilityId,
            string rejectionReason);

        List<EligibilityResult> GetManualReviewApplications();
    }
}
