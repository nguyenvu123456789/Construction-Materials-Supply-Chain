using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Application.Services.Implements
{
    public class EmailChannel : IEmailChannel
    {
        private readonly IConfiguration _cfg;
        public EmailChannel(IConfiguration cfg) { _cfg = cfg; }

        public async Task SendAsync(
            int partnerId,
            IEnumerable<string> toEmails,
            string subject,
            string body,
            CancellationToken ct = default)
        {
            var host = _cfg["Smtp:Host"];
            var port = int.TryParse(_cfg["Smtp:Port"], out var p) ? p : 587;
            var user = _cfg["Smtp:User"];
            var pass = _cfg["Smtp:Password"];
            var from = _cfg["Smtp:From"] ?? user;
            var enableSsl = bool.TryParse(_cfg["Smtp:EnableSsl"], out var ssl) ? ssl : true;

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(from) ||
                toEmails == null)
                return;

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = string.IsNullOrWhiteSpace(user) ? null : new NetworkCredential(user, pass)
            };

            using var msg = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject ?? string.Empty,
                Body = body ?? string.Empty,
                IsBodyHtml = true
            };

            foreach (var em in toEmails.Where(e => !string.IsNullOrWhiteSpace(e)).Distinct())
                msg.To.Add(em);

            if (msg.To.Count == 0) return;

            await client.SendMailAsync(msg, ct);
        }
    }
}
