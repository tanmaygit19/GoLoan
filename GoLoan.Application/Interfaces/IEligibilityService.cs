using GoLoan.Domain.Entities;

namespace GoLoan.Application.Interfaces
{
    public interface IEligibilityService
    {
        EligibilityResult CheckEligibility(int id);
    }
}
