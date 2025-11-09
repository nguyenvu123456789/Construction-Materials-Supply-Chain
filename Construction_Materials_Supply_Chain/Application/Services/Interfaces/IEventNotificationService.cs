using Application.DTOs;

namespace Application.Services.Interfaces
{
    public interface IEventNotificationService
    {
        EventNotifySettingDto Get(int partnerId, string eventType);
        IEnumerable<EventNotifySettingDto> GetAll(int partnerId);
        void Upsert(EventNotifySettingUpsertDto dto);
        void Trigger(EventNotifyTriggerDto dto);
    }
}
