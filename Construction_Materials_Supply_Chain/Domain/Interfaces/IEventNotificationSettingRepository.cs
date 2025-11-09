using Domain.Models;

namespace Domain.Interfaces
{
    public interface IEventNotificationSettingRepository
    {
        EventNotificationSetting GetOrCreate(int partnerId, string eventType);
        void Upsert(EventNotificationSetting setting);
        void ReplaceRoles(int settingId, IEnumerable<int> roleIds);
        EventNotificationSetting? Get(int partnerId, string eventType);
        IEnumerable<EventNotificationSetting> GetAllByPartner(int partnerId);
    }
}
