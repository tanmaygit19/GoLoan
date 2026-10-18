using GoLoan.Domain.Entities;

namespace GoLoan.Application.Interfaces
{
    public interface IScoreCardService
    {
        ScoreCard? GenerateScoreCard(int id);

        List<Customer> GetCustomers();
    }
}
