using System.Security.Claims;
using GoLoan.Application.DTO.Auth;
using GoLoan.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoLoan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly IOtpService _otpService;

        public AuthController(
            IAuthService authService,
            IJwtService jwtService,
            IOtpService otpService)
        {
            _authService = authService;
            _jwtService = jwtService;
            _otpService = otpService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });
            }

            var result = await _authService.Register(dto);
            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message });
            }

            return Ok(new { success = true, message = result.Message });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });
            }

            var result = await _authService.Login(dto);
            if (!result.Success)
            {
                return Unauthorized(new { success = false, message = result.Message });
            }

            if (result.RequiresTwoFactorSetup)
            {
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    requiresTwoFactorSetup = true,
                    preAuthToken = result.PreAuthToken,
                    qrCodeBase64 = result.QrCodeBase64,
                    tempSecret = result.TempSecret
                });
            }

            if (result.RequiresTwoFactor)
            {
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    requiresTwoFactor = true,
                    preAuthToken = result.PreAuthToken
                });
            }

            return Ok(new
            {
                success = true,
                message = result.Message,
                token = result.Token,
                tokenType = result.TokenType,
                tokenExpiry = result.TokenExpiry,
                userId = result.UserId,
                fullName = result.FullName,
                email = result.Email,
                role = result.Role,
                customerId = result.CustomerId
            });
        }

        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });
            }

            var result = await _authService.GoogleLogin(dto);
            if (!result.Success)
            {
                return Unauthorized(new { success = false, message = result.Message });
            }

            if (!string.IsNullOrEmpty(result.Token))
            {
                SetJwtCookie(result.Token);
            }

            return Ok(new
            {
                success = true,
                message = result.Message,
                token = result.Token,
                tokenType = result.TokenType,
                tokenExpiry = result.TokenExpiry,
                userId = result.UserId,
                fullName = result.FullName,
                email = result.Email,
                role = result.Role,
                customerId = result.CustomerId
            });
        }

        [HttpPost("Setup2FA")]
        public async Task<IActionResult> Setup2FA(
            [FromBody] VerifyOtpDTO dto,
            [FromHeader(Name = "X-PreAuth-Token")] string? preAuthToken,
            [FromHeader(Name = "X-Temp-Secret")] string? tempSecret)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed" });
            }

            if (string.IsNullOrEmpty(preAuthToken))
            {
                return Unauthorized(new { success = false, message = "Pre-auth token is required in X-PreAuth-Token header." });
            }

            if (string.IsNullOrEmpty(tempSecret))
            {
                return BadRequest(new { success = false, message = "Temp secret is required in X-Temp-Secret header." });
            }

            var userId = _jwtService.ValidatePreAuthToken(preAuthToken);
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Pre-authentication token is invalid or expired. Please login again." });
            }

            var isValid = _otpService.VerifyTotp(tempSecret, dto.Code);
            if (!isValid)
            {
                return Unauthorized(new { success = false, message = "Invalid code. Make sure you scanned the QR code and entered the current 6-digit code." });
            }

            await _authService.SaveTwoFaSecret(userId.Value, tempSecret);

            var user = await _authService.GetUserById(userId.Value);
            var token = _jwtService.GenerateToken(user!);

            SetJwtCookie(token);

            return Ok(new
            {
                success = true,
                message = "2FA setup complete. You are now logged in.",
                userId = user!.UserId,
                fullName = $"{user.FirstName} {user.LastName}",
                email = user.Email,
                role = user.Role?.RoleName ?? "Customer",
                customerId = user.CustomerId
            });
        }

        [HttpPost("Verify2FA")]
        public async Task<IActionResult> Verify2FA(
            [FromBody] VerifyOtpDTO dto,
            [FromHeader(Name = "X-PreAuth-Token")] string? preAuthToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed" });
            }

            if (string.IsNullOrEmpty(preAuthToken))
            {
                return Unauthorized(new { success = false, message = "Pre-auth token is required in X-PreAuth-Token header." });
            }

            var userId = _jwtService.ValidatePreAuthToken(preAuthToken);
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Pre-authentication token is invalid or expired. Please login again." });
            }

            var user = await _authService.GetUserById(userId.Value);
            if (user == null || string.IsNullOrEmpty(user.TwoFaSecret))
            {
                return Unauthorized(new { success = false, message = "2FA is not set up for this account." });
            }

            var isValid = _otpService.VerifyTotp(user.TwoFaSecret, dto.Code);
            if (!isValid)
            {
                return Unauthorized(new { success = false, message = "Invalid or expired code. Please try again." });
            }

            var token = _jwtService.GenerateToken(user);

            SetJwtCookie(token);

            return Ok(new
            {
                success = true,
                message = "Authentication successful",
                userId = user.UserId,
                fullName = $"{user.FirstName} {user.LastName}",
                email = user.Email,
                role = user.Role?.RoleName ?? "Customer",
                customerId = user.CustomerId
            });
        }

        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { success = false, message = "Invalid token credentials" });
            }

            var profile = await _authService.GetUserProfileAsync(userId);
            if (profile == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }

            return Ok(new { success = true, data = profile });
        }

        [Authorize]
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok(new { success = true, message = "Logged out successfully" });
        }

        private void SetJwtCookie(string token)
        {
            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60),
                Path = "/"
            });
        }
    }
}
