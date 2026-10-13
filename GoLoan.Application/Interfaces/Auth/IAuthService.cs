using GoLoan.Application.DTO.Auth;
using GoLoan.Domain.Entities;

namespace GoLoan.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> Register(RegisterDTO dto);
        Task<AuthResponseDTO> Login(LoginDTO dto);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int userId);
        Task<UserProfileDTO?> GetUserProfileAsync(int userId);
        Task SaveTwoFaSecret(int userId, string secret);
    }
}
