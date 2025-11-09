namespace Application.DTOs
{
    public enum NotificationTypeDto { Conversation = 1, Alert = 2 }

    public class CreateConversationRequestDto
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int PartnerId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime? DueAt { get; set; }
        public int[] RecipientUserIds { get; set; } = Array.Empty<int>();
        public int[] RecipientRoleIds { get; set; } = Array.Empty<int>();
    }

    public class CreateAlertRequestDto
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int PartnerId { get; set; }
        public int CreatedByUserId { get; set; }
        public bool RequireAcknowledge { get; set; }
        public int[] RecipientUserIds { get; set; } = Array.Empty<int>();
        public int[] RecipientRoleIds { get; set; } = Array.Empty<int>();
    }

    public class ReplyRequestDto
    {
        public int NotificationId { get; set; }
        public int PartnerId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = null!;
        public int? ParentReplyId { get; set; }
    }

    public class AckReadCloseRequestDto
    {
        public int NotificationId { get; set; }
        public int PartnerId { get; set; }
        public int UserId { get; set; }
    }

    public class NotificationReplyDto
    {
        public int NotificationReplyId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class NotificationResponseDto
    {
        public int NotificationId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int PartnerId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Type { get; set; }
        public bool? RequireAcknowledge { get; set; }
        public int Status { get; set; }
        public DateTime? DueAt { get; set; }
        public List<int> RecipientUserIds { get; set; } = new();
        public List<int> RecipientRoleIds { get; set; } = new();
        public List<NotificationReplyDto> Replies { get; set; } = new();
    }

    public class CrossPartnerAlertRequestDto
    {
        public int SenderPartnerId { get; set; }
        public int SenderUserId { get; set; }
        public int[] TargetPartnerIds { get; set; } = Array.Empty<int>();
        public int[] RecipientRoleIds { get; set; } = Array.Empty<int>();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool RequireAcknowledge { get; set; } = false;
        public bool SendZalo { get; set; } = false;
    }
}
