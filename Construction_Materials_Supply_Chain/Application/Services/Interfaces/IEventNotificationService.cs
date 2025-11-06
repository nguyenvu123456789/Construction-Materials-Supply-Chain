using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
