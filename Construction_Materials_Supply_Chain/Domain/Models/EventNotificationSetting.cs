namespace Domain.Models
{
    public partial class EventNotificationSetting
    {
        public int EventNotificationSettingId { get; set; }
        public int PartnerId { get; set; }
        public string EventType { get; set; } = null!;
        public bool RequireAcknowledge { get; set; } = false;
        public bool SendEmail { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public virtual Partner Partner { get; set; } = null!;
        public virtual ICollection<EventNotificationSettingRole> Roles { get; set; } = new List<EventNotificationSettingRole>();
    }

    public partial class EventNotificationSettingRole
    {
        public int EventNotificationSettingRoleId { get; set; }
        public int EventNotificationSettingId { get; set; }
        public int RoleId { get; set; }
        public virtual EventNotificationSetting Setting { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
