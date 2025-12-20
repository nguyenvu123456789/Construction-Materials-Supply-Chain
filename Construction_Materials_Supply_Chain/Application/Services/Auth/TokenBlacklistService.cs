using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Auth
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly IMemoryCache _cache;

        public TokenBlacklistService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void BlacklistToken(string jti, DateTime expiresAt)
        {
            var timeToLive = expiresAt - DateTime.Now;
            if (timeToLive <= TimeSpan.Zero) return;
            _cache.Set(jti, true, timeToLive);
        }

        public bool IsTokenBlacklisted(string jti)
        {
            return _cache.TryGetValue(jti, out _);
        }
    }
}
