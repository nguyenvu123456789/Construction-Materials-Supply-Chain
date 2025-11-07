using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ScmVlxdContext context) : base(context) { }

        public List<Notification> GetByPartner(int partnerId)
        {
            return _context.Notifications
                .Include(n => n.NotificationRecipients)
                .Include(n => n.NotificationRecipientRoles)
                .Include(n => n.NotificationReplies)
                .Where(n => n.PartnerId == partnerId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public Notification GetByIdWithRelations(int id, int partnerId)
        {
            return _context.Notifications
                .Include(n => n.NotificationRecipients)
                .Include(n => n.NotificationRecipientRoles)
                .Include(n => n.NotificationReplies)
                .FirstOrDefault(n => n.NotificationId == id && n.PartnerId == partnerId);
        }

        public void AddNotification(Notification entity)
        {
            _context.Notifications.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateNotification(Notification entity, int partnerId)
        {
            var exists = _context.Notifications.AsNoTracking().Any(n => n.NotificationId == entity.NotificationId && n.PartnerId == partnerId);
            if (!exists) return;
            _context.Notifications.Update(entity);
            _context.SaveChanges();
        }

        public void DeleteNotification(int id, int partnerId)
        {
            var n = _context.Notifications.FirstOrDefault(x => x.NotificationId == id && x.PartnerId == partnerId);
            if (n == null) return;
            _context.Notifications.Remove(n);
            _context.SaveChanges();
        }

        public void AddRecipients(int notificationId, int partnerId, IEnumerable<int> userIds)
        {
            var notif = _context.Notifications.FirstOrDefault(n => n.NotificationId == notificationId && n.PartnerId == partnerId);
            if (notif == null) return;

            var validUsers = _context.Users.Where(u => userIds.Contains(u.UserId) && u.PartnerId == partnerId).Select(u => u.UserId).ToList();
            var exists = _context.NotificationRecipients
                .Where(r => r.NotificationId == notificationId && r.PartnerId == partnerId && validUsers.Contains(r.UserId))
                .Select(r => r.UserId)
                .ToHashSet();

            var toAdd = validUsers.Where(uid => !exists.Contains(uid)).Select(uid => new NotificationRecipient
            {
                NotificationId = notificationId,
                PartnerId = partnerId,
                UserId = uid
            }).ToList();

            if (toAdd.Count == 0) return;
            _context.NotificationRecipients.AddRange(toAdd);
            _context.SaveChanges();
        }

        public void AddRecipientRoles(int notificationId, int partnerId, IEnumerable<int> roleIds)
        {
            var notif = _context.Notifications.FirstOrDefault(n => n.NotificationId == notificationId && n.PartnerId == partnerId);
            if (notif == null) return;

            var validRoles = _context.Roles.Where(r => roleIds.Contains(r.RoleId)).Select(r => r.RoleId).ToList();
            var exists = _context.NotificationRecipientRoles
                .Where(r => r.NotificationId == notificationId && r.PartnerId == partnerId && validRoles.Contains(r.RoleId))
                .Select(r => r.RoleId)
                .ToHashSet();

            var toAdd = validRoles.Where(rid => !exists.Contains(rid)).Select(rid => new NotificationRecipientRole
            {
                NotificationId = notificationId,
                PartnerId = partnerId,
                RoleId = rid
            }).ToList();

            if (toAdd.Count == 0) return;
            _context.NotificationRecipientRoles.AddRange(toAdd);
            _context.SaveChanges();
        }

        public void AddReply(NotificationReply entity, int partnerId)
        {
            var notif = _context.Notifications.FirstOrDefault(n => n.NotificationId == entity.NotificationId && n.PartnerId == partnerId);
            if (notif == null) return;

            var userValid = _context.Users.Any(u => u.UserId == entity.UserId && u.PartnerId == partnerId);
            if (!userValid) return;

            entity.PartnerId = partnerId;
            _context.NotificationReplies.Add(entity);
            _context.SaveChanges();
        }

        public void MarkRead(int notificationId, int partnerId, int userId)
        {
            var rec = _context.NotificationRecipients
                .FirstOrDefault(r => r.NotificationId == notificationId && r.PartnerId == partnerId && r.UserId == userId);
            if (rec == null) return;
            rec.IsRead = true;
            rec.ReadAt = System.DateTime.UtcNow;
            _context.NotificationRecipients.Update(rec);
            _context.SaveChanges();
        }

        public void Acknowledge(int notificationId, int partnerId, int userId)
        {
            var notif = _context.Notifications.FirstOrDefault(n => n.NotificationId == notificationId && n.PartnerId == partnerId && n.RequireAcknowledge == true);
            if (notif == null) return;

            var rec = _context.NotificationRecipients
                .FirstOrDefault(r => r.NotificationId == notificationId && r.PartnerId == partnerId && r.UserId == userId);
            if (rec == null) return;
            rec.IsAcknowledged = true;
            rec.AcknowledgedAt = System.DateTime.UtcNow;
            _context.NotificationRecipients.Update(rec);
            _context.SaveChanges();
        }

        public bool IsUserInPartner(int userId, int partnerId)
        {
            return _context.Users.Any(u => u.UserId == userId && u.PartnerId == partnerId);
        }

        public IEnumerable<int> GetUserIdsForRolesInPartner(IEnumerable<int> roleIds, int partnerId)
        {
            var userIds = _context.UserRoles
                .Where(ur => roleIds.Contains(ur.RoleId))
                .Select(ur => ur.UserId)
                .Distinct()
                .ToList();

            return _context.Users
                .Where(u => userIds.Contains(u.UserId) && u.PartnerId == partnerId)
                .Select(u => u.UserId)
                .ToList();
        }
    }
}
