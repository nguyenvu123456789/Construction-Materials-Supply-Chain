using Application.Services.Implements;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Persistence.Interceptors
{
    public class ZaloOptions { public string OaAccessToken { get; set; } = ""; public string OaId { get; set; } = ""; }

    public class ZaloChannel : IZaloChannel
    {
        private readonly HttpClient _http;
        private readonly ZaloOptions _opt;
        private readonly ScmVlxdContext _ctx;

        public ZaloChannel(HttpClient http, IOptions<ZaloOptions> opt, ScmVlxdContext ctx)
        { _http = http; _opt = opt.Value; _ctx = ctx; }

        public async Task SendAsync(int partnerId, IEnumerable<int> userIds, string title, string body, CancellationToken ct = default)
        {
            var zaloIds = await _ctx.Users.Where(u => userIds.Contains(u.UserId) && u.PartnerId == partnerId && u.ZaloUserId != null)
                .Select(u => u.ZaloUserId!).Distinct().ToListAsync(ct);
            foreach (var to in zaloIds)
            {
                var payload = new { recipient = new { user_id = to }, message = new { text = $"[{title}]\n{body}" } };
                using var req = new HttpRequestMessage(HttpMethod.Post, "https://openapi.zalo.me/v3.0/oa/message")
                { Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json") };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _opt.OaAccessToken);
                _ = await _http.SendAsync(req, ct);
            }
        }
    }
}
