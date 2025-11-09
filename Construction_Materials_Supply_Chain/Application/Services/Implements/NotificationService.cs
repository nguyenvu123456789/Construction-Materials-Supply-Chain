using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Application.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public NotificationResponseDto CreateConversation(CreateConversationRequestDto dto)
        {
            if (!_repo.IsUserInPartner(dto.CreatedByUserId, dto.PartnerId)) throw new InvalidOperationException();
            var n = new Notification
            {
                Title = dto.Title,
                Content = dto.Content,
                PartnerId = dto.PartnerId,
                UserId = dto.CreatedByUserId,
                CreatedAt = DateTime.UtcNow,
                Type = (int)NotificationTypeDto.Conversation,
                Status = 1,
                DueAt = dto.DueAt,
                RequireAcknowledge = false
            };
            _repo.AddNotification(n);
            AddRecipientsByUsers(n.NotificationId, n.PartnerId, dto.RecipientUserIds);
            AddRecipientsByRoles(n.NotificationId, n.PartnerId, dto.RecipientRoleIds);
            var loaded = _repo.GetByIdWithRelations(n.NotificationId, n.PartnerId);
            return _mapper.Map<NotificationResponseDto>(loaded);
        }

        public NotificationResponseDto CreateAlert(CreateAlertRequestDto dto)
        {
            if (!_repo.IsUserInPartner(dto.CreatedByUserId, dto.PartnerId)) throw new InvalidOperationException();
            var n = new Notification
            {
                Title = dto.Title,
                Content = dto.Content,
                PartnerId = dto.PartnerId,
                UserId = dto.CreatedByUserId,
                CreatedAt = DateTime.UtcNow,
                Type = (int)NotificationTypeDto.Alert,
                Status = 1,
                RequireAcknowledge = dto.RequireAcknowledge
            };
            _repo.AddNotification(n);
            AddRecipientsByUsers(n.NotificationId, n.PartnerId, dto.RecipientUserIds);
            AddRecipientsByRoles(n.NotificationId, n.PartnerId, dto.RecipientRoleIds);
            var loaded = _repo.GetByIdWithRelations(n.NotificationId, n.PartnerId);
            return _mapper.Map<NotificationResponseDto>(loaded);
        }

        public void AddRecipientsByUsers(int notificationId, int partnerId, IEnumerable<int> userIds)
        {
            if (userIds == null || !userIds.Any()) return;
            _repo.AddRecipients(notificationId, partnerId, userIds);
        }

        public void AddRecipientsByRoles(int notificationId, int partnerId, IEnumerable<int> roleIds)
        {
            if (roleIds == null || !roleIds.Any()) return;
            _repo.AddRecipientRoles(notificationId, partnerId, roleIds);
            var userIds = _repo.GetUserIdsForRolesInPartner(roleIds, partnerId);
            _repo.AddRecipients(notificationId, partnerId, userIds);
        }

        public void Reply(ReplyRequestDto dto)
        {
            var n = _repo.GetByIdWithRelations(dto.NotificationId, dto.PartnerId);
            if (n == null || n.Type != (int)NotificationTypeDto.Conversation) throw new InvalidOperationException();
            if (!_repo.IsUserInPartner(dto.UserId, dto.PartnerId)) throw new InvalidOperationException();
            var r = new NotificationReply
            {
                NotificationId = dto.NotificationId,
                PartnerId = dto.PartnerId,
                UserId = dto.UserId,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow,
                ParentReplyId = dto.ParentReplyId
            };
            _repo.AddReply(r, dto.PartnerId);
        }

        public void MarkRead(AckReadCloseRequestDto dto)
        {
            _repo.MarkRead(dto.NotificationId, dto.PartnerId, dto.UserId);
        }

        public void Acknowledge(AckReadCloseRequestDto dto)
        {
            _repo.Acknowledge(dto.NotificationId, dto.PartnerId, dto.UserId);
        }

        public void Close(AckReadCloseRequestDto dto)
        {
            var n = _repo.GetByIdWithRelations(dto.NotificationId, dto.PartnerId);
            if (n == null) return;
            n.Status = 2;
            _repo.UpdateNotification(n, dto.PartnerId);
        }

        public NotificationResponseDto GetById(int id, int partnerId)
        {
            var n = _repo.GetByIdWithRelations(id, partnerId);
            if (n == null) throw new KeyNotFoundException();
            return _mapper.Map<NotificationResponseDto>(n);
        }

        public List<NotificationResponseDto> GetByPartner(int partnerId)
        {
            var list = _repo.GetByPartner(partnerId);
            return list.Select(_mapper.Map<NotificationResponseDto>).ToList();
        }

        public void SendCrossPartnerAlert(CrossPartnerAlertRequestDto dto)
        {
            if (dto.SenderPartnerId <= 0 || dto.SenderUserId <= 0) return;
            if (!_repo.IsUserInPartner(dto.SenderUserId, dto.SenderPartnerId)) return;

            var roles = dto.RecipientRoleIds?.Where(r => r > 0).Distinct().ToArray() ?? Array.Empty<int>();
            if (roles.Length == 0) return;

            var targets = dto.TargetPartnerIds?.Where(p => p > 0 && p != dto.SenderPartnerId).Distinct().ToList() ?? new List<int>();
            if (targets.Count == 0) return;

            foreach (var partnerId in targets)
            {
                var recipients = _repo.GetUserIdsForRolesInPartner(roles, partnerId)?.Distinct().ToList() ?? new List<int>();
                if (recipients.Count == 0) continue;

                var createdBy = recipients[0];

                var req = new CreateAlertRequestDto
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    PartnerId = partnerId,
                    CreatedByUserId = createdBy,
                    RequireAcknowledge = dto.RequireAcknowledge,
                    RecipientRoleIds = roles
                };

                _ = CreateAlert(req);
            }
        }

        public List<NotificationResponseDto> GetForUser(int partnerId, int userId)
        {
            if (!_repo.IsUserInPartner(userId, partnerId)) return new List<NotificationResponseDto>();
            var list = _repo.GetForUser(partnerId, userId);
            return list.Select(_mapper.Map<NotificationResponseDto>).ToList();
        }

        public int CountUnreadForUser(int partnerId, int userId)
        {
            if (!_repo.IsUserInPartner(userId, partnerId)) return 0;
            return _repo.CountUnreadForUser(partnerId, userId);
        }
    }

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

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from) || toEmails == null) return;

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
