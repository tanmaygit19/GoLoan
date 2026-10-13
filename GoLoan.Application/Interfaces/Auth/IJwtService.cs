using GoLoan.Domain.Entities;

namespace GoLoan.Application.Interfaces.Auth
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GeneratePreAuthToken(User user);
        int? ValidatePreAuthToken(string token);
    }
}
