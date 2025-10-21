using Application.Services.Auth;
using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Auth
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly SigningCredentials _creds;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresMinutes;

        public JwtTokenGenerator(SecurityKey key, string issuer, string audience, int expiresMinutes)
        {
            _creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            _issuer = issuer;
            _audience = audience;
            _expiresMinutes = expiresMinutes;
        }

        public string GenerateToken(User user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
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
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_expiresMinutes),
                signingCredentials: _creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}