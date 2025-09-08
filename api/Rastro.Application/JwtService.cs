using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rastro.Application.Abstractions;
using Rastro.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rastro.Application
{
    public class JwtService : IJwtService
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;        // from config
        private readonly TimeSpan _ttl;      // e.g. 15 minutes

        public JwtService(IConfiguration cfg)
        {
            _issuer = cfg["Jwt:Issuer"]!;
            _audience = cfg["Jwt:Audience"]!;
            _key = cfg["Jwt:Key"]!;
            _ttl = TimeSpan.FromDays(int.Parse(cfg["Jwt:AccessTokenMinutes"] ?? "15"));
        }

        public string GenerateToken(User user)
        {
            var now = DateTime.UtcNow;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.DisplayName),
                new Claim("email_verified", user.EmailVerified ? "true" : "false"),
                new Claim(JwtRegisteredClaimNames.Iat,
                        new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_ttl),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
