using Application.Constants.Enums;
using Application.Constants.Messages;
using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository notificationRepository;
        private readonly INotificationEventSettingRepository eventSettingRepository;
        private readonly INotificationLowStockRuleRepository lowStockRuleRepository;
        private readonly IUserRepository userRepository;
        private readonly IInventoryRepository inventoryRepository;
        private readonly IEmailChannel emailChannel;
        private readonly IMapper mapper;

        public NotificationService(
            INotificationRepository notificationRepository,
            INotificationEventSettingRepository eventSettingRepository,
            INotificationLowStockRuleRepository lowStockRuleRepository,
            IUserRepository userRepository,
            IInventoryRepository inventoryRepository,
            IEmailChannel emailChannel,
            IMapper mapper)
        {
            this.notificationRepository = notificationRepository;
            this.eventSettingRepository = eventSettingRepository;
            this.lowStockRuleRepository = lowStockRuleRepository;
            this.userRepository = userRepository;
            this.inventoryRepository = inventoryRepository;
            this.emailChannel = emailChannel;
            this.mapper = mapper;
        }

        // ================= CORE NOTIFICATIONS =====================

        public NotificationResponseDto CreateConversation(CreateConversationRequestDto request)
        {
            var notification = new Notification
            {
                PartnerId = request.PartnerId,
                UserId = request.CreatedByUserId,
                Title = request.Title,
                Content = request.Content,
                Type = (int)NotificationType.Conversation,
                RequireAcknowledge = request.RequireAcknowledge,
                Status = 1,
                CreatedAt = DateTime.UtcNow
            };

            notificationRepository.AddNotification(notification);

            var recipients = ResolveRecipientUserIds(
                request.PartnerId,
                request.RecipientUserIds,
                request.RecipientRoleIds);

            if (recipients.Any())
            {
                notificationRepository.AddRecipients(notification.NotificationId, request.PartnerId, recipients);
                notificationRepository.AddRecipientRoles(notification.NotificationId, request.PartnerId, request.RecipientRoleIds);
                SendNotificationEmails(notification, recipients);
            }

            return mapper.Map<NotificationResponseDto>(notification);
        }

        public NotificationResponseDto CreateAlert(CreateAlertRequestDto request)
        {
            var notification = new Notification
            {
                PartnerId = request.PartnerId,
                UserId = request.CreatedByUserId,
                Title = request.Title,
                Content = request.Content,
                Type = (int)NotificationType.Alert,
                RequireAcknowledge = request.RequireAcknowledge,
                Status = 1,
                CreatedAt = DateTime.UtcNow
            };

            notificationRepository.AddNotification(notification);

            var recipients = ResolveRecipientUserIds(
                request.PartnerId,
                request.RecipientUserIds,
                request.RecipientRoleIds);

            if (recipients.Any())
            {
                notificationRepository.AddRecipients(notification.NotificationId, request.PartnerId, recipients);
                notificationRepository.AddRecipientRoles(notification.NotificationId, request.PartnerId, request.RecipientRoleIds);
                SendNotificationEmails(notification, recipients);
            }

            return mapper.Map<NotificationResponseDto>(notification);
        }

        public void AddRecipientsByUsers(int notificationId, int partnerId, int[] userIds)
        {
            var ids = userIds?.Where(x => x > 0).Distinct().ToArray() ?? Array.Empty<int>();
            if (ids.Length == 0) return;

            notificationRepository.AddRecipients(notificationId, partnerId, ids);
            var notification = notificationRepository.GetByIdWithRelations(notificationId, partnerId);
            SendNotificationEmails(notification, ids);
        }

        public void AddRecipientsByRoles(int notificationId, int partnerId, int[] roleIds)
        {
            var roles = roleIds?.Where(x => x > 0).Distinct().ToArray() ?? Array.Empty<int>();
            if (roles.Length == 0) return;

            var userIds = notificationRepository
                .GetUserIdsForRolesInPartner(roles, partnerId)
                .Where(x => x > 0)
                .Distinct()
                .ToArray();

            if (userIds.Length == 0) return;

            notificationRepository.AddRecipientRoles(notificationId, partnerId, roles);
            notificationRepository.AddRecipients(notificationId, partnerId, userIds);

            var notification = notificationRepository.GetByIdWithRelations(notificationId, partnerId);
            SendNotificationEmails(notification, userIds);
        }

        public void Reply(ReplyRequestDto request)
        {
            var reply = new NotificationReply
            {
                NotificationId = request.NotificationId,
                PartnerId = request.PartnerId,
                UserId = request.UserId,
                Message = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            notificationRepository.AddReply(reply, request.PartnerId);
        }

        public void MarkRead(AckReadCloseRequestDto request)
        {
            notificationRepository.MarkRead(request.NotificationId, request.PartnerId, request.UserId);
        }

        public void Acknowledge(AckReadCloseRequestDto request)
        {
            notificationRepository.Acknowledge(request.NotificationId, request.PartnerId, request.UserId);
        }

        public void Close(AckReadCloseRequestDto request)
        {
            var notification = notificationRepository.GetByIdWithRelations(request.NotificationId, request.PartnerId);
            notification.Status = 2;
            notificationRepository.UpdateNotification(notification, request.PartnerId);
        }

        public NotificationResponseDto? GetById(int id, int partnerId)
        {
            var notification = notificationRepository.GetByIdWithRelations(id, partnerId);
            return notification == null ? null : mapper.Map<NotificationResponseDto>(notification);
        }

        public List<NotificationResponseDto> GetByPartner(int partnerId)
        {
            var list = notificationRepository.GetByPartner(partnerId);
            return mapper.Map<List<NotificationResponseDto>>(list);
        }

        public List<NotificationResponseDto> GetForUser(int partnerId, int userId)
        {
            var list = notificationRepository.GetForUser(partnerId, userId);
            return mapper.Map<List<NotificationResponseDto>>(list);
        }

        public int CountUnreadForUser(int partnerId, int userId)
        {
            return notificationRepository.CountUnreadForUser(partnerId, userId);
        }

        public void SendCrossPartnerAlert(CrossPartnerAlertRequestDto request)
        {
            var roleIds = request.RecipientRoleIds ?? Array.Empty<int>();

            foreach (var targetPartnerId in request.TargetPartnerIds ?? Array.Empty<int>())
            {
                var notification = new Notification
                {
                    PartnerId = targetPartnerId,
                    UserId = request.SenderUserId,
                    Title = request.Title,
                    Content = request.Content,
                    Type = (int)NotificationType.CrossPartnerAlert,
                    RequireAcknowledge = request.RequireAcknowledge,
                    Status = 1,
                    CreatedAt = DateTime.UtcNow
                };

                notificationRepository.AddNotification(notification);

                var recipients = notificationRepository
                    .GetUserIdsForRolesInPartner(roleIds, targetPartnerId)
                    .Where(x => x > 0)
                    .Distinct()
                    .ToArray();

                if (recipients.Any())
                {
                    notificationRepository.AddRecipients(notification.NotificationId, targetPartnerId, recipients);
                    notificationRepository.AddRecipientRoles(notification.NotificationId, targetPartnerId, roleIds);
                    SendNotificationEmails(notification, recipients);
                }
            }
        }

        // ================= EVENT RULES ============================

        public EventNotifySettingDto? GetEventSetting(int partnerId, string eventType)
        {
            var setting = eventSettingRepository.Get(partnerId, eventType);
            return setting == null ? null : mapper.Map<EventNotifySettingDto>(setting);
        }

        public IEnumerable<EventNotifySettingDto> GetEventSettings(int partnerId)
        {
            var settings = eventSettingRepository.GetAllByPartner(partnerId);
            return settings.Select(x => mapper.Map<EventNotifySettingDto>(x));
        }

        public void UpsertEventSetting(EventNotifySettingUpsertDto request)
        {
            var entity = eventSettingRepository.Get(request.PartnerId, request.EventType)
                         ?? new EventNotificationSetting();

            entity.PartnerId = request.PartnerId;
            entity.EventType = request.EventType;
            entity.RequireAcknowledge = request.RequireAcknowledge;
            entity.SendEmail = request.SendEmail;
            entity.IsActive = request.IsActive;

            eventSettingRepository.Upsert(entity);

            eventSettingRepository.ReplaceRoles(
                entity.EventNotificationSettingId,
                entity.PartnerId,
                request.RoleIds ?? Array.Empty<int>());
        }

        public void ToggleEventSetting(int settingId, int partnerId, bool isActive)
        {
            eventSettingRepository.SetActive(settingId, isActive, partnerId);
        }

        public void DeleteEventSetting(int settingId, int partnerId)
        {
            eventSettingRepository.Delete(settingId, partnerId);
        }

        public void TriggerEvent(EventNotifyTriggerDto request)
        {
            var setting = eventSettingRepository.GetOrCreate(request.PartnerId, request.EventType);
            if (!setting.IsActive) return;

            var roleIds = request.OverrideRoleIds ??
                          setting.Roles.Select(r => r.RoleId).ToArray();

            roleIds = roleIds.Where(x => x > 0).Distinct().ToArray();
            if (roleIds.Length == 0) return;

            var requireAcknowledge = request.OverrideRequireAcknowledge ?? setting.RequireAcknowledge;

            var userIds = notificationRepository
                .GetUserIdsForRolesInPartner(roleIds, request.PartnerId)
                .Where(x => x > 0)
                .Distinct()
                .ToArray();

            if (userIds.Length == 0) return;

            var createdBy = request.CreatedByUserId.HasValue &&
                            notificationRepository.IsUserInPartner(request.CreatedByUserId.Value, request.PartnerId)
                ? request.CreatedByUserId.Value
                : userIds.First();

            var alertRequest = new CreateAlertRequestDto
            {
                PartnerId = request.PartnerId,
                CreatedByUserId = createdBy,
                Title = request.Title,
                Content = request.Content,
                RequireAcknowledge = requireAcknowledge,
                RecipientUserIds = userIds,
                RecipientRoleIds = roleIds
            };

            CreateAlert(alertRequest);
        }

        public void Trigger(EventNotifyTriggerDto request)
        {
            TriggerEvent(request);
        }

        // ================= LOW STOCK RULES ========================

        public IEnumerable<AlertRuleUpdateDto> GetLowStockRules(int partnerId)
        {
            var rules = lowStockRuleRepository.GetByPartner(partnerId);
            return rules.Select(x => mapper.Map<AlertRuleUpdateDto>(x));
        }

        public int CreateLowStockRule(AlertRuleCreateDto request)
        {
            var rule = mapper.Map<InventoryAlertRule>(request);
            lowStockRuleRepository.Add(rule);

            if (request.RoleIds.Any())
                lowStockRuleRepository.ReplaceRoles(rule.InventoryAlertRuleId, request.PartnerId, request.RoleIds);

            if (request.UserIds.Any())
                lowStockRuleRepository.ReplaceUsers(rule.InventoryAlertRuleId, request.PartnerId, request.UserIds);

            return rule.InventoryAlertRuleId;
        }

        public void UpdateLowStockRule(AlertRuleUpdateDto request)
        {
            var rule = lowStockRuleRepository.Get(request.RuleId, request.PartnerId);
            if (rule == null) return;

            rule.WarehouseId = request.WarehouseId;
            rule.MaterialId = request.MaterialId;
            rule.MinQuantity = request.MinQuantity;
            rule.CriticalMinQuantity = request.CriticalMinQuantity;
            rule.SendEmail = request.SendEmail;
            rule.IsActive = request.IsActive;
            rule.RecipientMode = (AlertRecipientMode)request.RecipientMode;

            lowStockRuleRepository.Update(rule);
            lowStockRuleRepository.ReplaceRoles(rule.InventoryAlertRuleId, request.PartnerId, request.RoleIds ?? Array.Empty<int>());
            lowStockRuleRepository.ReplaceUsers(rule.InventoryAlertRuleId, request.PartnerId, request.UserIds ?? Array.Empty<int>());
        }

        public void ToggleLowStockRule(int ruleId, int partnerId, bool isActive)
        {
            lowStockRuleRepository.SetActive(ruleId, isActive, partnerId);
        }

        public void DeleteLowStockRule(int ruleId, int partnerId)
        {
            lowStockRuleRepository.Delete(ruleId, partnerId);
        }

        public void RunLowStockAlerts(RunAlertDto request)
        {
            var rules = lowStockRuleRepository.GetActiveByPartner(request.PartnerId);
            if (rules.Count == 0) return;

            foreach (var rule in rules)
            {
                var inventories = GetInventoriesForRule(rule, request.PartnerId);
                foreach (var inventory in inventories)
                {
                    var quantity = inventory.Quantity ?? 0m;
                    if (quantity > rule.MinQuantity) continue;

                    var recipients = ResolveRecipientsForLowStockRule(rule, request.PartnerId, inventory)
                        .Where(x => x > 0)
                        .Distinct()
                        .ToArray();

                    if (!recipients.Any())
                        continue;

                    var title = NotificationMessages.LowStockTitle;
                    var body = string.Format(
                        NotificationMessages.LowStockBodyFormat,
                        inventory.MaterialId,
                        inventory.WarehouseId,
                        quantity,
                        rule.MinQuantity);

                    var notification = new Notification
                    {
                        PartnerId = request.PartnerId,
                        Title = title,
                        Content = body,
                        Type = (int)NotificationType.LowStockAlert,
                        RequireAcknowledge = true,
                        Status = 1,
                        CreatedAt = DateTime.UtcNow,
                        UserId = recipients.First()
                    };

                    notificationRepository.AddNotification(notification);
                    notificationRepository.AddRecipients(notification.NotificationId, request.PartnerId, recipients);

                    if (rule.SendEmail)
                        SendNotificationEmails(notification, recipients);
                }
            }
        }

        // ================= PRIVATE HELPERS ========================

        private int[] ResolveRecipientUserIds(int partnerId, int[] userIds, int[] roleIds)
        {
            var direct = userIds?.Where(x => x > 0).Distinct().ToArray() ?? Array.Empty<int>();

            var fromRoles = roleIds == null || roleIds.Length == 0
                ? Array.Empty<int>()
                : notificationRepository.GetUserIdsForRolesInPartner(roleIds, partnerId)
                    .Where(x => x > 0)
                    .Distinct()
                    .ToArray();

            return direct.Concat(fromRoles).Distinct().ToArray();
        }

        private void SendNotificationEmails(Notification notification, IEnumerable<int> userIds)
        {
            var ids = userIds?.Where(x => x > 0).Distinct().ToList();
            if (ids == null || ids.Count == 0) return;

            var emails = userRepository.GetEmailsByUserIds(ids);
            var subject = NotificationMessages.NewNotificationEmailSubjectPrefix + notification.Title;
            var body = string.IsNullOrWhiteSpace(notification.Content)
                ? NotificationMessages.DefaultNewNotificationEmailBody
                : notification.Content;

            emailChannel.SendAsync(notification.PartnerId, emails, subject, body);
        }

        private List<Inventory> GetInventoriesForRule(InventoryAlertRule rule, int partnerId)
        {
            if (rule.WarehouseId.HasValue)
            {
                var inv = inventoryRepository.GetByMaterialId(rule.MaterialId, rule.WarehouseId.Value);
                return inv == null ? new List<Inventory>() : new List<Inventory> { inv };
            }

            return inventoryRepository
                .GetAllByPartnerId(partnerId)
                .Where(x => x.MaterialId == rule.MaterialId)
                .ToList();
        }

        private IEnumerable<int> ResolveRecipientsForLowStockRule(InventoryAlertRule rule, int partnerId, Inventory inventory)
        {
            if (rule.RecipientMode == AlertRecipientMode.Manager)
            {
                if (inventory.Warehouse?.Manager != null)
                    return new[] { inventory.Warehouse.Manager.UserId };
                return Array.Empty<int>();
            }

            if (rule.RecipientMode == AlertRecipientMode.Roles)
            {
                var roleIds = rule.Roles.Select(r => r.RoleId).ToArray();
                if (roleIds.Length == 0) return Array.Empty<int>();

                return notificationRepository.GetUserIdsForRolesInPartner(roleIds, partnerId);
            }

            return rule.Users.Select(u => u.UserId).ToArray();
        }
    }
}
