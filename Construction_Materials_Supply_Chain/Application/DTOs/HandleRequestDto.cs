namespace Application.DTOs
{
    public class HandleRequestDto
    {
        public int HandledBy { get; set; }
        public string HandledByName { get; set; } = "";
        public string ActionType { get; set; } = "";
        public string? Note { get; set; }
        public DateTime HandledAt { get; set; }
    }
}
