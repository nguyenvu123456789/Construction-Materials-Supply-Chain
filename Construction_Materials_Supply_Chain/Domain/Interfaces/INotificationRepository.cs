using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface INotificationRepository
    {
        List<Notification> GetByPartner(int partnerId);
        Notification GetByIdWithRelations(int id, int partnerId);

        void AddNotification(Notification entity);
        void UpdateNotification(Notification entity, int partnerId);
        void DeleteNotification(int id, int partnerId);

        void AddRecipients(int notificationId, int partnerId, IEnumerable<int> userIds);
        void AddRecipientRoles(int notificationId, int partnerId, IEnumerable<int> roleIds);
        void AddReply(NotificationReply entity, int partnerId);

        void MarkRead(int notificationId, int partnerId, int userId);
        void Acknowledge(int notificationId, int partnerId, int userId);

        List<Notification> GetForUser(int partnerId, int userId);
        int CountUnreadForUser(int partnerId, int userId);

        bool IsUserInPartner(int userId, int partnerId);
        IEnumerable<int> GetUserIdsForRolesInPartner(IEnumerable<int> roleIds, int partnerId);
    }
}
