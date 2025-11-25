using Domain.Models;

namespace Domain.Interfaces
{
    public interface INotificationRepository
    {
        List<Notification> GetByPartner(int partnerId);
        Notification? GetByIdWithRelations(int id, int partnerId);

        void AddNotification(Notification entity);
        void UpdateNotification(Notification entity, int partnerId);
        void DeleteNotification(int id, int partnerId);

        void AddRecipients(int notificationId, int partnerId, IEnumerable<int> userIds);
        void AddRecipientRoles(int notificationId, int partnerId, IEnumerable<int> roleIds);

        void AddReply(NotificationReply reply, int partnerId);
        void MarkRead(int notificationId, int partnerId, int userId);
        void Acknowledge(int notificationId, int partnerId, int userId);

        List<Notification> GetForUser(int partnerId, int userId);
        int CountUnreadForUser(int partnerId, int userId);

        bool IsUserInPartner(int userId, int partnerId);
        IEnumerable<int> GetUserIdsForRolesInPartner(IEnumerable<int> roleIds, int partnerId);
    }

    public interface INotificationEventSettingRepository
    {
        EventNotificationSetting GetOrCreate(int partnerId, string eventType);
        EventNotificationSetting? Get(int partnerId, string eventType);
        IEnumerable<EventNotificationSetting> GetAllByPartner(int partnerId);
        void Upsert(EventNotificationSetting setting);
        void ReplaceRoles(int id, int partnerId, IEnumerable<int> roleIds);
        void SetActive(int settingId, bool isActive, int partnerId);
        void Delete(int settingId, int partnerId);
    }

    public interface INotificationLowStockRuleRepository
    {
        InventoryAlertRule Add(InventoryAlertRule rule);
        void Update(InventoryAlertRule rule);
        InventoryAlertRule? Get(int id, int partnerId);
        List<InventoryAlertRule> GetByPartner(int partnerId);
        List<InventoryAlertRule> GetActiveByPartner(int partnerId);
        void SetActive(int id, bool active, int partnerId);
        void Delete(int id, int partnerId);
        void ReplaceRoles(int id, int partnerId, IEnumerable<int> roleIds);
        void ReplaceUsers(int id, int partnerId, IEnumerable<int> userIds);
        void ReplaceMaterials(int id, int partnerId, IEnumerable<int> materialIds);
    }
}
