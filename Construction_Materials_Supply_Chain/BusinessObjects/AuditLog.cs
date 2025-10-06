namespace BusinessObjects
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public string EntityName { get; set; } = null!;
        public int EntityId { get; set; }
        public string Action { get; set; } = null!;
        public string? Changes { get; set; }
        public int? UserId { get; set; }
        public virtual User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
