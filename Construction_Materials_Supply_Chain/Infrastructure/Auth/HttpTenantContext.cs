using Application.DTOs.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Auth
{
    public sealed class HttpTenantContext : ITenantContext
    {
        private readonly IHttpContextAccessor _http;

        public HttpTenantContext(IHttpContextAccessor http) => _http = http;

        public int? PartnerId
        {
            get
            {
                var s = _http.HttpContext?.User.FindFirstValue("partnerId");
                return int.TryParse(s, out var id) ? id : (int?)null;
            }
        }

        public IEnumerable<string> Roles =>
            _http.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();

        public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }
}
