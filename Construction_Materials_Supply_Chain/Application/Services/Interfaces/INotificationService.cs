using Application.DTOs;

namespace Application.Services.Interfaces
{
    public interface INotificationService
    {
        // Core notifications
        NotificationResponseDto CreateConversation(CreateConversationRequestDto request);
        NotificationResponseDto CreateAlert(CreateAlertRequestDto request);
        void AddRecipientsByUsers(int notificationId, int partnerId, int[] userIds);
        void AddRecipientsByRoles(int notificationId, int partnerId, int[] roleIds);
        void Reply(ReplyRequestDto request);
        void MarkRead(AckReadCloseRequestDto request);
        void Acknowledge(AckReadCloseRequestDto request);
        void Close(AckReadCloseRequestDto request);
        NotificationResponseDto? GetById(int id, int partnerId);
        List<NotificationResponseDto> GetByPartner(int partnerId);
        List<NotificationResponseDto> GetForUser(int partnerId, int userId);
        int CountUnreadForUser(int partnerId, int userId);
        void SendCrossPartnerAlert(CrossPartnerAlertRequestDto request);

        // Event notification rules
        EventNotifySettingDto? GetEventSetting(int partnerId, string eventType);
        IEnumerable<EventNotifySettingDto> GetEventSettings(int partnerId);
        void UpsertEventSetting(EventNotifySettingUpsertDto request);
        void ToggleEventSetting(int settingId, int partnerId, bool isActive);
        void DeleteEventSetting(int settingId, int partnerId);
        void TriggerEvent(EventNotifyTriggerDto request);
        void Trigger(EventNotifyTriggerDto request);

        // Low stock alert rules
        IEnumerable<AlertRuleUpdateDto> GetLowStockRules(int partnerId);
        int CreateLowStockRule(AlertRuleCreateDto request);
        void UpdateLowStockRule(AlertRuleUpdateDto request);
        void ToggleLowStockRule(int ruleId, int partnerId, bool isActive);
        void DeleteLowStockRule(int ruleId, int partnerId);
        void RunLowStockAlerts(RunAlertDto request);
    }
}
