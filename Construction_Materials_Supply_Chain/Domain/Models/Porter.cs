namespace Domain.Models
{
    public class Porter
    {
        public int PorterId { get; set; }
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public bool Active { get; set; } = true;
    }
}
