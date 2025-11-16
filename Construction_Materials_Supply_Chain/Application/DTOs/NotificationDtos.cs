namespace Application.DTOs
{
    public class CreateConversationRequestDto
    {
        public int PartnerId { get; set; }
        public int CreatedByUserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool RequireAcknowledge { get; set; } = false;
        public int[] RecipientUserIds { get; set; } = Array.Empty<int>();
        public int[] RecipientRoleIds { get; set; } = Array.Empty<int>();
    }

    public class CreateAlertRequestDto
    {
        public int PartnerId { get; set; }
        public int CreatedByUserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool RequireAcknowledge { get; set; } = false;
        public int[] RecipientUserIds { get; set; } = Array.Empty<int>();
        public int[] RecipientRoleIds { get; set; } = Array.Empty<int>();
    }

    public class NotificationResponseDto
    {
        public int NotificationId { get; set; }
        public int PartnerId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int Type { get; set; }
        public bool RequireAcknowledge { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<int> RecipientUserIds { get; set; } = Array.Empty<int>();
        public IEnumerable<int> RecipientRoleIds { get; set; } = Array.Empty<int>();
        public IEnumerable<NotificationReplyDto> Replies { get; set; } = new List<NotificationReplyDto>();
    }

    public class NotificationReplyDto
    {
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class AckReadCloseRequestDto
    {
        public int NotificationId { get; set; }
        public int PartnerId { get; set; }
        public int UserId { get; set; }
    }

    public class ReplyRequestDto
    {
        public int NotificationId { get; set; }
        public int PartnerId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
    }

    public class CrossPartnerAlertRequestDto
    {
        public int SenderPartnerId { get; set; }
        public int SenderUserId { get; set; }
        public int[] TargetPartnerIds { get; set; } = Array.Empty<int>();
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool RequireAcknowledge { get; set; }
        public int[] RecipientRoleIds { get; set; } = Array.Empty<int>();
    }

    public class EventNotifySettingDto
    {
        public int SettingId { get; set; }
        public int PartnerId { get; set; }
        public string EventType { get; set; } = null!;
        public bool RequireAcknowledge { get; set; }
        public bool SendEmail { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public int[] RoleIds { get; set; } = Array.Empty<int>();
    }

    public class EventNotifySettingUpsertDto
    {
        public int PartnerId { get; set; }
        public string EventType { get; set; } = null!;
        public bool RequireAcknowledge { get; set; }
        public bool SendEmail { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public int[] RoleIds { get; set; } = Array.Empty<int>();
    }

    public class EventNotifyTriggerDto
    {
        public int PartnerId { get; set; }
        public int? CreatedByUserId { get; set; }
        public string EventType { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int[]? OverrideRoleIds { get; set; }
        public bool? OverrideRequireAcknowledge { get; set; }
    }

    public class AlertRuleCreateDto
    {
        public int PartnerId { get; set; }
        public int? WarehouseId { get; set; }
        public int MaterialId { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal? CriticalMinQuantity { get; set; }
        public bool SendEmail { get; set; } = true;
        public int RecipientMode { get; set; }
        public int[] RoleIds { get; set; } = Array.Empty<int>();
        public int[] UserIds { get; set; } = Array.Empty<int>();
    }

    public class AlertRuleUpdateDto
    {
        public int RuleId { get; set; }
        public int PartnerId { get; set; }
        public int? WarehouseId { get; set; }
        public int MaterialId { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal? CriticalMinQuantity { get; set; }
        public bool SendEmail { get; set; } = true;
        public int RecipientMode { get; set; }
        public bool IsActive { get; set; } = true;
        public int[] RoleIds { get; set; } = Array.Empty<int>();
        public int[] UserIds { get; set; } = Array.Empty<int>();
    }

    public class RunAlertDto
    {
        public int PartnerId { get; set; }
    }
}
