namespace Application.DTOs
{
    public class EventNotifySettingDto
    {
        public int PartnerId { get; set; }
        public string EventType { get; set; } = null!;
        public bool RequireAcknowledge { get; set; }
        public bool SendZalo { get; set; }
        public int[] RoleIds { get; set; } = Array.Empty<int>();
    }

    public class EventNotifySettingUpsertDto
    {
        public int PartnerId { get; set; }
        public string EventType { get; set; } = null!;
        public bool RequireAcknowledge { get; set; }
        public bool SendZalo { get; set; }
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
}
