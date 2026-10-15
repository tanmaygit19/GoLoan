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
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IMapper _mapper;

        public AuthService(
            AppDbContext db,
            IJwtService jwtService,
            IOtpService otpService,
            IGoogleAuthService googleAuthService,
            IMapper mapper)
        {
            _db = db;
            _jwtService = jwtService;
            _otpService = otpService;
            _googleAuthService = googleAuthService;
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

            int? linkedCustomerId = null;
            if (dto.RoleId == 1)
            {
                var existingCustomer = await _db.Customers
                    .FirstOrDefaultAsync(c => c.Email == dto.Email || (dto.Pan != null && c.Pan == dto.Pan));

                if (existingCustomer != null)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = "A customer profile with this email or PAN already exists"
                    };
                }

                var customer = new Customer
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age ?? 25,
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    MobileNo = dto.Mobile,
                    Pan = dto.Pan ?? "ABCDE1234F",
                    AadhaarNo = dto.AadhaarNo ?? "123456789012",
                    EmploymentType = dto.EmploymentType ?? "Salaried",
                    MonthlyIncome = dto.MonthlyIncome ?? 50000,
                    MonthlyInvestment = dto.MonthlyInvestment ?? 0,
                    IsEmailVerified = false
                };

                _db.Customers.Add(customer);
                await _db.SaveChangesAsync();

                linkedCustomerId = customer.CustomerId;
            }

            var user = _mapper.Map<User>(dto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.CustomerId = linkedCustomerId;
            user.IsTwoFaEnabled = false;
            user.IsEmailVerified = false;
            user.TwoFaSecret = null;
            user.AuthProvider = "Local";

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new AuthResponseDTO
            {
                Success = true,
                Message = "Registration successful. Please login to continue.",
                CustomerId = linkedCustomerId,
                UserId = user.UserId
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

        public async Task<AuthResponseDTO> GoogleLogin(GoogleLoginDTO dto)
        {
            var googleUser = await _googleAuthService.ValidateGoogleToken(dto.IdToken);
            if (googleUser == null)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid or expired Google token"
                };
            }

            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.GoogleSubjectId == googleUser.GoogleSubjectId || u.Email == googleUser.Email);

            if (user == null)
            {
                var customerRole = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == "Customer")
                                   ?? await _db.Roles.FirstOrDefaultAsync();

                if (customerRole == null)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = "Default Customer role not found in database."
                    };
                }

                var existingCustomer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == googleUser.Email);
                int? linkedCustomerId = null;

                if (existingCustomer != null)
                {
                    linkedCustomerId = existingCustomer.CustomerId;
                }
                else if (customerRole.RoleId == 1 || customerRole.RoleName == "Customer")
                {
                    var newCustomer = new Customer
                    {
                        FirstName = !string.IsNullOrWhiteSpace(googleUser.FirstName) ? googleUser.FirstName : "Google",
                        LastName = !string.IsNullOrWhiteSpace(googleUser.LastName) ? googleUser.LastName : "User",
                        Age = 25,
                        Email = googleUser.Email,
                        Password = "",
                        MobileNo = "9876543210",
                        Pan = "ABCDE1234F",
                        AadhaarNo = "123456789012",
                        EmploymentType = "Salaried",
                        MonthlyIncome = 50000,
                        MonthlyInvestment = 0,
                        IsEmailVerified = googleUser.EmailVerified
                    };
                    _db.Customers.Add(newCustomer);
                    await _db.SaveChangesAsync();
                    linkedCustomerId = newCustomer.CustomerId;
                }

                user = new User
                {
                    FirstName = !string.IsNullOrWhiteSpace(googleUser.FirstName) ? googleUser.FirstName : "Google",
                    LastName = !string.IsNullOrWhiteSpace(googleUser.LastName) ? googleUser.LastName : "User",
                    Email = googleUser.Email,
                    Mobile = "9876543210",
                    Password = "",
                    RoleId = customerRole.RoleId,
                    Role = customerRole,
                    CustomerId = linkedCustomerId,
                    IsTwoFaEnabled = false,
                    IsEmailVerified = googleUser.EmailVerified,
                    GoogleSubjectId = googleUser.GoogleSubjectId,
                    AuthProvider = "Google"
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }
            else
            {
                bool updated = false;

                if (user.CustomerId == null && (user.Role?.RoleName == "Customer" || user.RoleId == 1))
                {
                    var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == user.Email);
                    if (customer == null)
                    {
                        customer = new Customer
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Age = 25,
                            Email = user.Email,
                            Password = "",
                            MobileNo = !string.IsNullOrEmpty(user.Mobile) ? user.Mobile : "9876543210",
                            Pan = "ABCDE1234F",
                            AadhaarNo = "123456789012",
                            EmploymentType = "Salaried",
                            MonthlyIncome = 50000,
                            MonthlyInvestment = 0,
                            IsEmailVerified = user.IsEmailVerified
                        };
                        _db.Customers.Add(customer);
                        await _db.SaveChangesAsync();
                    }
                    user.CustomerId = customer.CustomerId;
                    updated = true;
                }

                if (string.IsNullOrEmpty(user.GoogleSubjectId))
                {
                    user.GoogleSubjectId = googleUser.GoogleSubjectId;
                    updated = true;
                }
                if (!user.IsEmailVerified && googleUser.EmailVerified)
                {
                    user.IsEmailVerified = true;
                    updated = true;
                }
                if (updated)
                {
                    await _db.SaveChangesAsync();
                }
            }

            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDTO
            {
                Success = true,
                Message = "Google authentication successful",
                Token = token,
                TokenType = "Bearer",
                TokenExpiry = DateTime.UtcNow.AddMinutes(60),
                UserId = user.UserId,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = user.Role?.RoleName ?? "Customer",
                CustomerId = user.CustomerId
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
