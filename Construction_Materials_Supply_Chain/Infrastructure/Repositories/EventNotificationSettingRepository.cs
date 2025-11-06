using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EventNotificationSettingRepository : IEventNotificationSettingRepository
    {
        private readonly ScmVlxdContext _ctx;
        public EventNotificationSettingRepository(ScmVlxdContext ctx) { _ctx = ctx; }

        public EventNotificationSetting GetOrCreate(int partnerId, string eventType)
        {
            var s = _ctx.EventNotificationSettings
                .Include(x => x.Roles)
                .FirstOrDefault(x => x.PartnerId == partnerId && x.EventType == eventType);
            if (s != null) return s;
            s = new EventNotificationSetting { PartnerId = partnerId, EventType = eventType, RequireAcknowledge = false, SendZalo = false };
            _ctx.EventNotificationSettings.Add(s);
            _ctx.SaveChanges();
            return s;
        }

        public void Upsert(EventNotificationSetting setting)
        {
            var exist = _ctx.EventNotificationSettings.FirstOrDefault(x => x.PartnerId == setting.PartnerId && x.EventType == setting.EventType);
            if (exist == null)
            {
                _ctx.EventNotificationSettings.Add(setting);
            }
            else
            {
                exist.RequireAcknowledge = setting.RequireAcknowledge;
                exist.SendZalo = setting.SendZalo;
                _ctx.EventNotificationSettings.Update(exist);
            }
            _ctx.SaveChanges();
        }

        public void ReplaceRoles(int settingId, IEnumerable<int> roleIds)
        {
            var existed = _ctx.EventNotificationSettingRoles.Where(x => x.EventNotificationSettingId == settingId).ToList();
            _ctx.EventNotificationSettingRoles.RemoveRange(existed);
            var toAdd = roleIds.Distinct().Select(rid => new EventNotificationSettingRole { EventNotificationSettingId = settingId, RoleId = rid }).ToList();
            if (toAdd.Count > 0) _ctx.EventNotificationSettingRoles.AddRange(toAdd);
            _ctx.SaveChanges();
        }

        public EventNotificationSetting? Get(int partnerId, string eventType)
        {
            return _ctx.EventNotificationSettings
                .Include(x => x.Roles)
                .FirstOrDefault(x => x.PartnerId == partnerId && x.EventType == eventType);
        }

        public IEnumerable<EventNotificationSetting> GetAllByPartner(int partnerId)
        {
            return _ctx.EventNotificationSettings.Include(x => x.Roles).Where(x => x.PartnerId == partnerId).ToList();
        }
    }
}
