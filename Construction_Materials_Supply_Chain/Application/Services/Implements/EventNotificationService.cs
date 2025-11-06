using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Interface;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implements
{
    public class EventNotificationService : IEventNotificationService
    {
        private readonly IEventNotificationSettingRepository _settings;
        private readonly INotificationService _noti;
        private readonly INotificationRepository _repoNoti;
        private readonly IZaloChannel _zalo;
        private readonly IUserRepository _userRepo;

        public EventNotificationService(IEventNotificationSettingRepository settings, INotificationService noti, INotificationRepository repoNoti, IZaloChannel zalo)
        {
            _settings = settings; 
            _noti = noti; 
            _repoNoti = repoNoti; 
            _zalo = zalo;
        }

        public EventNotifySettingDto Get(int partnerId, string eventType)
        {
            var s = _settings.GetOrCreate(partnerId, eventType);
            return new EventNotifySettingDto
            {
                PartnerId = s.PartnerId,
                EventType = s.EventType,
                RequireAcknowledge = s.RequireAcknowledge,
                SendZalo = s.SendZalo,
                RoleIds = s.Roles.Select(r => r.RoleId).ToArray()
            };
        }

        public IEnumerable<EventNotifySettingDto> GetAll(int partnerId)
        {
            return _settings.GetAllByPartner(partnerId).Select(s => new EventNotifySettingDto
            {
                PartnerId = s.PartnerId,
                EventType = s.EventType,
                RequireAcknowledge = s.RequireAcknowledge,
                SendZalo = s.SendZalo,
                RoleIds = s.Roles.Select(r => r.RoleId).ToArray()
            });
        }

        public void Upsert(EventNotifySettingUpsertDto dto)
        {
            var s = _settings.GetOrCreate(dto.PartnerId, dto.EventType);
            s.RequireAcknowledge = dto.RequireAcknowledge;
            s.SendZalo = dto.SendZalo;
            _settings.Upsert(s);
            _settings.ReplaceRoles(s.EventNotificationSettingId, dto.RoleIds ?? System.Array.Empty<int>());
        }

        public void Trigger(EventNotifyTriggerDto dto)
        {
            var s = _settings.GetOrCreate(dto.PartnerId, dto.EventType);

            var roles = (dto.OverrideRoleIds ?? (s.Roles?.Select(r => r.RoleId).ToArray() ?? Array.Empty<int>()))
                .Where(x => x > 0)
                .Distinct()
                .ToArray();
            if (roles.Length == 0) return;

            var requireAck = dto.OverrideRequireAcknowledge ?? s.RequireAcknowledge;

            var recipients = _repoNoti.GetUserIdsForRolesInPartner(roles, dto.PartnerId)?
                .Where(uid => uid > 0)
                .Distinct()
                .ToList() ?? new List<int>();
            if (recipients.Count == 0) return;

            int createdBy = dto.CreatedByUserId.HasValue && _repoNoti.IsUserInPartner(dto.CreatedByUserId.Value, dto.PartnerId)
                ? dto.CreatedByUserId.Value
                : recipients[0];

            var alert = new CreateAlertRequestDto
            {
                Title = dto.Title,
                Content = dto.Content,
                PartnerId = dto.PartnerId,
                RequireAcknowledge = requireAck,
                RecipientRoleIds = roles,
                CreatedByUserId = createdBy
            };

            var res = _noti.CreateAlert(alert);

            if (s.SendZalo && recipients.Count > 0)
                _ = _zalo.SendAsync(dto.PartnerId, recipients, dto.Title, dto.Content);
        }
    }
}
