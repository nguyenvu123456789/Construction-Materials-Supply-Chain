using Application.DTOs.Common;
using Application.Services.Auth;
using Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Auth
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly SigningCredentials _creds;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresMinutes;

        public JwtTokenGenerator(IOptions<JwtOptions> opts)
        {
            var o = opts.Value ?? throw new ArgumentNullException(nameof(opts));
            if (string.IsNullOrWhiteSpace(o.Key)) throw new InvalidOperationException("JwtOptions.Key is missing");
            _creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(o.Key)), SecurityAlgorithms.HmacSha256);
            _issuer = o.Issuer!;
            _audience = o.Audience!;
            _expiresMinutes = o.ExpiresMinutes <= 0 ? 60 : o.ExpiresMinutes;
        }

        public string GenerateToken(User user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("username", user.UserName ?? string.Empty)
            };

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            if (user.PartnerId != null)
            {
                claims.Add(new Claim("partnerId", user.PartnerId.Value.ToString()));
                if (user.Partner != null)
                {
                    if (!string.IsNullOrWhiteSpace(user.Partner.PartnerName))
                        claims.Add(new Claim("partnerName", user.Partner.PartnerName));
                    var partnerTypeName = user.Partner.PartnerType?.TypeName;
                    if (!string.IsNullOrWhiteSpace(partnerTypeName))
                        claims.Add(new Claim("partnerType", partnerTypeName));
                }
            }

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_expiresMinutes),
                signingCredentials: _creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}