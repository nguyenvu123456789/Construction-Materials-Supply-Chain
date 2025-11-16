using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NotificationRepository
        : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ScmVlxdContext context) : base(context) { }

        public List<Notification> GetByPartner(int partnerId)
        {
            return _context.Notifications
                .Include(x => x.NotificationRecipients)
                .Include(x => x.NotificationRecipientRoles)
                .Include(x => x.NotificationReplies)
                .Where(x => x.PartnerId == partnerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public Notification? GetByIdWithRelations(int id, int partnerId)
        {
            return _context.Notifications
                .Include(x => x.NotificationRecipients)
                .Include(x => x.NotificationRecipientRoles)
                .Include(x => x.NotificationReplies)
                .FirstOrDefault(x => x.NotificationId == id && x.PartnerId == partnerId);
        }

        public void AddNotification(Notification entity)
        {
            _context.Notifications.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateNotification(Notification entity, int partnerId)
        {
            var existing = _context.Notifications
                .FirstOrDefault(x => x.NotificationId == entity.NotificationId && x.PartnerId == partnerId);
            if (existing == null) return;

            existing.Title = entity.Title;
            existing.Content = entity.Content;
            existing.Type = entity.Type;
            existing.RequireAcknowledge = entity.RequireAcknowledge;
            existing.Status = entity.Status;
            existing.DueAt = entity.DueAt;
            existing.UserId = entity.UserId;

            _context.SaveChanges();
        }

        public void DeleteNotification(int id, int partnerId)
        {
            var notification = _context.Notifications
                .Include(x => x.NotificationRecipients)
                .Include(x => x.NotificationRecipientRoles)
                .Include(x => x.NotificationReplies)
                .FirstOrDefault(x => x.NotificationId == id && x.PartnerId == partnerId);

            if (notification == null) return;

            if (notification.NotificationRecipients?.Count > 0)
                _context.NotificationRecipients.RemoveRange(notification.NotificationRecipients);

            if (notification.NotificationRecipientRoles?.Count > 0)
                _context.NotificationRecipientRoles.RemoveRange(notification.NotificationRecipientRoles);

            if (notification.NotificationReplies?.Count > 0)
                _context.NotificationReplies.RemoveRange(notification.NotificationReplies);

            _context.Notifications.Remove(notification);
            _context.SaveChanges();
        }

        public void AddRecipients(int notificationId, int partnerId, IEnumerable<int> userIds)
        {
            var validUserIds = _context.Users
                .Where(u => u.PartnerId == partnerId && userIds.Contains(u.UserId))
                .Select(u => u.UserId)
                .ToList();

            if (validUserIds.Count == 0) return;

            var existing = _context.NotificationRecipients
                .Where(r => r.NotificationId == notificationId && r.PartnerId == partnerId)
                .Select(r => r.UserId)
                .ToHashSet();

            var toAdd = validUserIds
                .Where(id => !existing.Contains(id))
                .Select(id => new NotificationRecipient
                {
                    NotificationId = notificationId,
                    PartnerId = partnerId,
                    UserId = id,
                    IsRead = false,
                    IsAcknowledged = false
                })
                .ToList();

            if (toAdd.Count > 0)
                _context.NotificationRecipients.AddRange(toAdd);

            _context.SaveChanges();
        }

        public void AddRecipientRoles(int notificationId, int partnerId, IEnumerable<int> roleIds)
        {
            var validRoleIds = _context.Roles
                .Where(r => roleIds.Contains(r.RoleId))
                .Select(r => r.RoleId)
                .ToList();

            if (validRoleIds.Count == 0) return;

            var existing = _context.NotificationRecipientRoles
                .Where(r => r.NotificationId == notificationId && r.PartnerId == partnerId)
                .Select(r => r.RoleId)
                .ToHashSet();

            var toAdd = validRoleIds
                .Where(id => !existing.Contains(id))
                .Select(id => new NotificationRecipientRole
                {
                    NotificationId = notificationId,
                    PartnerId = partnerId,
                    RoleId = id
                })
                .ToList();

            if (toAdd.Count > 0)
                _context.NotificationRecipientRoles.AddRange(toAdd);

            _context.SaveChanges();
        }

        public void AddReply(NotificationReply reply, int partnerId)
        {
            reply.PartnerId = partnerId;
            _context.NotificationReplies.Add(reply);
            _context.SaveChanges();
        }

        public void MarkRead(int notificationId, int partnerId, int userId)
        {
            var recipient = _context.NotificationRecipients
                .FirstOrDefault(r => r.NotificationId == notificationId &&
                                     r.PartnerId == partnerId &&
                                     r.UserId == userId);
            if (recipient == null) return;

            recipient.IsRead = true;
            _context.SaveChanges();
        }

        public void Acknowledge(int notificationId, int partnerId, int userId)
        {
            var recipient = _context.NotificationRecipients
                .FirstOrDefault(r => r.NotificationId == notificationId &&
                                     r.PartnerId == partnerId &&
                                     r.UserId == userId);
            if (recipient == null) return;

            recipient.IsAcknowledged = true;
            _context.SaveChanges();
        }

        public List<Notification> GetForUser(int partnerId, int userId)
        {
            return _context.Notifications
                .Include(x => x.NotificationRecipients)
                .Include(x => x.NotificationRecipientRoles)
                .Include(x => x.NotificationReplies)
                .Where(x => x.PartnerId == partnerId &&
                            (x.NotificationRecipients.Any(r => r.UserId == userId) || x.UserId == userId))
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public int CountUnreadForUser(int partnerId, int userId)
        {
            return _context.NotificationRecipients
                .Count(r => r.PartnerId == partnerId && r.UserId == userId && !r.IsRead);
        }

        public bool IsUserInPartner(int userId, int partnerId)
        {
            return _context.Users.Any(u => u.UserId == userId && u.PartnerId == partnerId);
        }

        public IEnumerable<int> GetUserIdsForRolesInPartner(IEnumerable<int> roleIds, int partnerId)
        {
            var roleIdList = roleIds.Distinct().ToList();
            if (roleIdList.Count == 0) return Enumerable.Empty<int>();

            return _context.UserRoles
                .Where(ur => ur.User.PartnerId == partnerId && roleIdList.Contains(ur.RoleId))
                .Select(ur => ur.UserId)
                .Distinct()
                .ToList();
        }
    }

    public class NotificationEventSettingRepository
    : GenericRepository<EventNotificationSetting>, INotificationEventSettingRepository
    {
        public NotificationEventSettingRepository(ScmVlxdContext context) : base(context) { }

        public EventNotificationSetting GetOrCreate(int partnerId, string eventType)
        {
            var setting = _context.EventNotificationSettings
                .Include(x => x.Roles)
                .FirstOrDefault(x => x.PartnerId == partnerId && x.EventType == eventType);

            if (setting != null) return setting;

            setting = new EventNotificationSetting
            {
                PartnerId = partnerId,
                EventType = eventType,
                SendEmail = true,
                IsActive = true
            };

            _context.EventNotificationSettings.Add(setting);
            _context.SaveChanges();
            return setting;
        }

        public EventNotificationSetting? Get(int partnerId, string eventType)
        {
            return _context.EventNotificationSettings
                .Include(x => x.Roles)
                .FirstOrDefault(x => x.PartnerId == partnerId && x.EventType == eventType);
        }

        public IEnumerable<EventNotificationSetting> GetAllByPartner(int partnerId)
        {
            return _context.EventNotificationSettings
                .Include(x => x.Roles)
                .Where(x => x.PartnerId == partnerId)
                .ToList();
        }

        public void Upsert(EventNotificationSetting setting)
        {
            if (setting.EventNotificationSettingId == 0)
                _context.EventNotificationSettings.Add(setting);
            else
                _context.EventNotificationSettings.Update(setting);

            _context.SaveChanges();
        }

        public void ReplaceRoles(int id, int partnerId, IEnumerable<int> roleIds)
        {
            var existing = _context.InventoryAlertRuleRoles
                .Where(x => x.InventoryAlertRuleId == id);
            _context.InventoryAlertRuleRoles.RemoveRange(existing);

            var roleIdList = roleIds?
                .Where(r => r > 0)
                .Distinct()
                .ToList() ?? new List<int>();

            if (roleIdList.Count == 0)
            {
                _context.SaveChanges();
                return;
            }

            var validRoleIds = _context.Roles
                .Where(r => roleIdList.Contains(r.RoleId))
                .Select(r => r.RoleId)
                .ToList();

            if (validRoleIds.Count == 0)
            {
                _context.SaveChanges();
                return;
            }

            var insert = validRoleIds.Select(r => new InventoryAlertRuleRole
            {
                InventoryAlertRuleId = id,
                RoleId = r
            });

            _context.InventoryAlertRuleRoles.AddRange(insert);
            _context.SaveChanges();
        }

        public void SetActive(int id, bool active, int partnerId)
        {
            var setting = _context.EventNotificationSettings
                .FirstOrDefault(x => x.EventNotificationSettingId == id && x.PartnerId == partnerId);

            if (setting == null) return;

            setting.IsActive = active;
            _context.SaveChanges();
        }

        public void Delete(int id, int partnerId)
        {
            var setting = _context.EventNotificationSettings
                .Include(x => x.Roles)
                .FirstOrDefault(x => x.EventNotificationSettingId == id && x.PartnerId == partnerId);

            if (setting == null) return;

            _context.EventNotificationSettingRoles.RemoveRange(setting.Roles);
            _context.EventNotificationSettings.Remove(setting);
            _context.SaveChanges();
        }
    }

    public class NotificationLowStockRuleRepository
    : GenericRepository<InventoryAlertRule>, INotificationLowStockRuleRepository
    {
        public NotificationLowStockRuleRepository(ScmVlxdContext context) : base(context) { }

        public InventoryAlertRule Add(InventoryAlertRule rule)
        {
            _context.InventoryAlertRules.Add(rule);
            _context.SaveChanges();
            return rule;
        }

        public void Update(InventoryAlertRule rule)
        {
            _context.InventoryAlertRules.Update(rule);
            _context.SaveChanges();
        }

        public InventoryAlertRule? Get(int id, int partnerId)
        {
            return _context.InventoryAlertRules
                .Include(x => x.Roles)
                .Include(x => x.Users)
                .FirstOrDefault(x => x.InventoryAlertRuleId == id && x.PartnerId == partnerId);
        }

        public List<InventoryAlertRule> GetByPartner(int partnerId)
        {
            return _context.InventoryAlertRules
                .Include(x => x.Roles)
                .Include(x => x.Users)
                .Where(x => x.PartnerId == partnerId)
                .ToList();
        }

        public List<InventoryAlertRule> GetActiveByPartner(int partnerId)
        {
            return _context.InventoryAlertRules
                .Include(x => x.Roles)
                .Include(x => x.Users)
                .Where(x => x.PartnerId == partnerId && x.IsActive)
                .ToList();
        }

        public void SetActive(int id, bool active, int partnerId)
        {
            var rule = _context.InventoryAlertRules
                .FirstOrDefault(x => x.InventoryAlertRuleId == id && x.PartnerId == partnerId);

            if (rule == null) return;

            rule.IsActive = active;
            _context.SaveChanges();
        }

        public void Delete(int id, int partnerId)
        {
            var rule = _context.InventoryAlertRules
                .Include(x => x.Roles)
                .Include(x => x.Users)
                .FirstOrDefault(x => x.InventoryAlertRuleId == id && x.PartnerId == partnerId);

            if (rule == null) return;

            _context.InventoryAlertRuleRoles.RemoveRange(rule.Roles);
            _context.InventoryAlertRuleUsers.RemoveRange(rule.Users);
            _context.InventoryAlertRules.Remove(rule);
            _context.SaveChanges();
        }

        public void ReplaceRoles(int id, int partnerId, IEnumerable<int> roleIds)
        {
            var existing = _context.InventoryAlertRuleRoles
                .Where(x => x.InventoryAlertRuleId == id);

            _context.InventoryAlertRuleRoles.RemoveRange(existing);

            var insert = roleIds.Select(r => new InventoryAlertRuleRole
            {
                InventoryAlertRuleId = id,
                RoleId = r
            });

            _context.InventoryAlertRuleRoles.AddRange(insert);
            _context.SaveChanges();
        }

        public void ReplaceUsers(int id, int partnerId, IEnumerable<int> userIds)
        {
            var existing = _context.InventoryAlertRuleUsers
                .Where(x => x.InventoryAlertRuleId == id);

            _context.InventoryAlertRuleUsers.RemoveRange(existing);

            var validUserIds = _context.Users
                .Where(u => u.PartnerId == partnerId && userIds.Contains(u.UserId))
                .Select(u => u.UserId)
                .ToList();

            var insert = validUserIds.Select(uid => new InventoryAlertRuleUser
            {
                InventoryAlertRuleId = id,
                UserId = uid
            });

            _context.InventoryAlertRuleUsers.AddRange(insert);
            _context.SaveChanges();
        }
    }
}