using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GoLoan.Application.DTO.Auth;
using GoLoan.Application.Interfaces.Auth;
using GoLoan.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GoLoan.Infrastructure.Repositories.Auth
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettingsDTO _settings;

        public JwtService(IOptions<JwtSettingsDTO> options)
        {
            _settings = options.Value;
        }

        public string GenerateToken(User user)
        {
            var roleName = user.Role?.RoleName ?? "Customer";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("Purpose", "Access")
            };

            if (user.CustomerId.HasValue)
                claims.Add(new Claim("CustomerId", user.CustomerId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GeneratePreAuthToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Purpose", "2FA")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int? ValidatePreAuthToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
                var principal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings.Issuer,
                    ValidAudience = _settings.Audience,
                    IssuerSigningKey = key
                }, out _);

                var purpose = principal.FindFirst("Purpose")?.Value;
                if (purpose != "2FA") return null;

                var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(idClaim, out int id) ? id : null;
            }
            catch { return null; }
        }
    }
}
