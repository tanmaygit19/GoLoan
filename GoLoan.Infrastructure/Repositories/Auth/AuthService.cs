using AutoMapper;
using GoLoan.Application.DTO.Auth;
using GoLoan.Application.Interfaces.Auth;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoLoan.Infrastructure.Repositories.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IJwtService _jwtService;
        private readonly IOtpService _otpService;
        private readonly IMapper _mapper;

        public AuthService(
            AppDbContext db,
            IJwtService jwtService,
            IOtpService otpService,
            IMapper mapper)
        {
            _db = db;
            _jwtService = jwtService;
            _otpService = otpService;
            _mapper = mapper;
        }

        public async Task<AuthResponseDTO> Register(RegisterDTO dto)
        {
            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "An account with this email already exists"
                };
            }

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.RoleId == dto.RoleId);
            if (role == null)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid role specified"
                };
            }

            var user = _mapper.Map<User>(dto);

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.IsTwoFaEnabled = false;
            user.IsEmailVerified = false;
            user.TwoFaSecret = null;
            user.AuthProvider = "Local";

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new AuthResponseDTO
            {
                Success = true,
                Message = "Registration successful. Please login to continue."
            };
        }

        public async Task<AuthResponseDTO> Login(LoginDTO dto)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            if (user.AuthProvider == "Google" && string.IsNullOrEmpty(user.Password))
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "This account uses Google login. Please sign in with Google."
                };
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            if (!passwordValid)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            var preAuthToken = _jwtService.GeneratePreAuthToken(user);

            if (string.IsNullOrEmpty(user.TwoFaSecret))
            {
                var secret = _otpService.GenerateSecret();
                var qrCode = _otpService.GenerateQrCodeBase64(secret, user.Email);

                return new AuthResponseDTO
                {
                    Success = true,
                    Message = "Scan the QR code with Google Authenticator and submit the 6-digit code",
                    RequiresTwoFactorSetup = true,
                    PreAuthToken = preAuthToken,
                    QrCodeBase64 = qrCode,
                    TempSecret = secret
                };
            }

            return new AuthResponseDTO
            {
                Success = true,
                Message = "Enter the 6-digit code from Google Authenticator",
                RequiresTwoFactor = true,
                PreAuthToken = preAuthToken
            };
        }

        public async Task<UserProfileDTO?> GetUserProfileAsync(int userId)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserProfileDTO>(user);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task SaveTwoFaSecret(int userId, string secret)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                user.TwoFaSecret = secret;
                user.IsTwoFaEnabled = true;
                await _db.SaveChangesAsync();
            }
        }
    }
}
