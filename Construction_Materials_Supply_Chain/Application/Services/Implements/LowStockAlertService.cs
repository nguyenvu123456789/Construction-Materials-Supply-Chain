using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services.Implements
{
    public interface IZaloChannel
    {
        Task SendAsync(int partnerId, IEnumerable<int> userIds, string title, string body, CancellationToken ct = default);
    }

    public class LowStockAlertService : ILowStockAlertService
    {
        private readonly IInventoryAlertRuleRepository _rules;
        private readonly IInventoryRepository _inventories;
        private readonly INotificationRepository _notifs;
        private readonly IZaloChannel _zalo;

        public LowStockAlertService(IInventoryAlertRuleRepository rules, IInventoryRepository inventories, INotificationRepository notifs, IZaloChannel zalo)
        {
            _rules = rules; _inventories = inventories; _notifs = notifs; _zalo = zalo;
        }

        public int Create(AlertRuleCreateDto dto)
        {
            var rule = new InventoryAlertRule
            {
                PartnerId = dto.PartnerId,
                WarehouseId = dto.WarehouseId,
                MaterialId = dto.MaterialId,
                MinQuantity = dto.MinQuantity,
                CriticalMinQuantity = dto.CriticalMinQuantity,
                SendZalo = dto.SendZalo,
                RecipientMode = (AlertRecipientMode)dto.RecipientMode,
                IsActive = true
            };
            rule = _rules.Add(rule);
            if (dto.RoleIds?.Any() == true) _rules.ReplaceRoles(rule.InventoryAlertRuleId, dto.PartnerId, dto.RoleIds);
            if (dto.UserIds?.Any() == true) _rules.ReplaceUsers(rule.InventoryAlertRuleId, dto.PartnerId, dto.UserIds);
            return rule.InventoryAlertRuleId;
        }

        public void Update(AlertRuleUpdateDto dto)
        {
            var rule = _rules.Get(dto.RuleId, dto.PartnerId);
            if (rule == null) return;
            rule.WarehouseId = dto.WarehouseId;
            rule.MaterialId = dto.MaterialId;
            rule.MinQuantity = dto.MinQuantity;
            rule.CriticalMinQuantity = dto.CriticalMinQuantity;
            rule.SendZalo = dto.SendZalo;
            rule.RecipientMode = (AlertRecipientMode)dto.RecipientMode;
            rule.IsActive = dto.IsActive;
            _rules.Update(rule);
            _rules.ReplaceRoles(rule.InventoryAlertRuleId, dto.PartnerId, dto.RoleIds ?? Array.Empty<int>());
            _rules.ReplaceUsers(rule.InventoryAlertRuleId, dto.PartnerId, dto.UserIds ?? Array.Empty<int>());
        }

        public void Toggle(int ruleId, int partnerId, bool isActive) => _rules.SetActive(ruleId, isActive, partnerId);

        public void RunOnce(int partnerId)
        {
            var rules = _rules.GetActiveByPartner(partnerId);
            foreach (var r in rules)
            {
                List<Inventory> inventories;

                if (r.WarehouseId.HasValue)
                {
                    var inv = _inventories.GetByMaterialId(r.MaterialId, r.WarehouseId.Value);
                    inventories = inv != null ? new List<Inventory> { inv } : new List<Inventory>();
                }
                else
                {
                    inventories = _inventories
                        .GetAllByPartnerId(partnerId)
                        .Where(i => i.MaterialId == r.MaterialId)
                        .ToList();
                }

                foreach (var inv in inventories)
                {
                    var qty = inv.Quantity ?? 0m;
                    if (qty > r.MinQuantity) continue;

                    var title = "Cảnh báo tồn kho thấp";
                    var body = $"Vật tư #{inv.MaterialId} tại kho #{inv.WarehouseId} còn {qty} (ngưỡng {r.MinQuantity}).";

                    var notif = new Notification
                    {
                        Title = title,
                        Content = body,
                        PartnerId = partnerId,
                        UserId = 0,
                        CreatedAt = DateTime.UtcNow,
                        Type = 2,
                        RequireAcknowledge = true,
                        Status = 1
                    };
                    _notifs.AddNotification(notif);

                    IEnumerable<int> recipients = Enumerable.Empty<int>();

                    if (r.RecipientMode == AlertRecipientMode.Manager)
                    {
                        if (inv.Warehouse?.Manager != null)
                            recipients = new[] { inv.Warehouse.Manager.UserId };
                    }
                    else if (r.RecipientMode == AlertRecipientMode.Roles)
                    {
                        recipients = _notifs.GetUserIdsForRolesInPartner(r.Roles.Select(x => x.RoleId), partnerId);
                    }
                    else
                    {
                        recipients = r.Users.Select(x => x.UserId);
                    }

                    var userIds = recipients.Distinct().ToList();
                    if (userIds.Count > 0)
                    {
                        _notifs.AddRecipients(notif.NotificationId, partnerId, userIds);
                        if (r.SendZalo) _ = _zalo.SendAsync(partnerId, userIds, title, body);
                    }
                }
            }
        }
    }
}
