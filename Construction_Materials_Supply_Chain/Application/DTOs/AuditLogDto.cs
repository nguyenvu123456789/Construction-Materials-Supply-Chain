namespace Application.DTOs
{
    public class AuditLogDto
    {
        public int AuditLogId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? EntityName { get; set; }
        public string? Action { get; set; }
        public string? Changes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
