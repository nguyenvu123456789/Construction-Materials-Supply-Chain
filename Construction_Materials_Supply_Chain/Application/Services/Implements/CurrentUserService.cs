using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services.Implements
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;

        public CurrentUserService(IHttpContextAccessor http)
        {
            _http = http;
        }

        public int? UserId
        {
            get
            {
                var id = _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? _http.HttpContext?.User?.FindFirst("sub")?.Value;
                if (int.TryParse(id, out var i)) return i;
                return null;
            }
        }

        public string? UserName =>
            _http.HttpContext?.User?.Identity?.Name
            ?? _http.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
    }
}
