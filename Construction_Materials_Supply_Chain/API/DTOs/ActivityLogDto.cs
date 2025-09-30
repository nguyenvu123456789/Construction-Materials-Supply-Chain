namespace API.DTOs
{
    public class ActivityLogDto
    {
        public int LogId { get; set; }
        public int? UserId { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public int? EntityId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
