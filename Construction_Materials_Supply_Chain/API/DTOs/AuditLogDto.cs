namespace API.DTOs
{
    public class AuditLogDto
    {
        public int AuditLogId { get; set; }
        public string EntityName { get; set; } = null!;
        public int EntityId { get; set; }
        public string Action { get; set; } = null!;
        public string? Changes { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
