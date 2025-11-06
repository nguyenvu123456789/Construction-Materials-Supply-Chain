namespace Domain.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int PartnerId { get; set; }
        public int Type { get; set; }
        public bool? RequireAcknowledge { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? DueAt { get; set; }
        public virtual User? User { get; set; }
        public virtual Partner Partner { get; set; } = null!;
        public virtual ICollection<NotificationRecipient> NotificationRecipients { get; set; } = new List<NotificationRecipient>();
        public virtual ICollection<NotificationRecipientRole> NotificationRecipientRoles { get; set; } = new List<NotificationRecipientRole>();
        public virtual ICollection<NotificationReply> NotificationReplies { get; set; } = new List<NotificationReply>();
    }

    public partial class NotificationRecipient
    {
        public int NotificationRecipientId { get; set; }
        public int NotificationId { get; set; }
        public int PartnerId { get; set; }
        public int UserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsAcknowledged { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public virtual Notification Notification { get; set; } = null!;
        public virtual Partner Partner { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }

    public partial class NotificationRecipientRole
    {
        public int NotificationRecipientRoleId { get; set; }
        public int NotificationId { get; set; }
        public int PartnerId { get; set; }
        public int RoleId { get; set; }
        public virtual Notification Notification { get; set; } = null!;
        public virtual Partner Partner { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }

    public partial class NotificationReply
    {
        public int NotificationReplyId { get; set; }
        public int NotificationId { get; set; }
        public int PartnerId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? ParentReplyId { get; set; }
        public virtual Notification Notification { get; set; } = null!;
        public virtual Partner Partner { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}