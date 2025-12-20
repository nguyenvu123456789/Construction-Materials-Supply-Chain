using Domain.Models;

namespace Application.Services.Auth
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, IEnumerable<string> roles);
    }
    public interface ITokenBlacklistService
    {
        void BlacklistToken(string jti, DateTime expiresAt);
        bool IsTokenBlacklisted(string jti);
    }
}
