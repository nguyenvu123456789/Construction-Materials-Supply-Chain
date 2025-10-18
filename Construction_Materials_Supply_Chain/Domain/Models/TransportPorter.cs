namespace Domain.Models
{
    public class TransportPorter
    {
        public int TransportId { get; set; }
        public int PorterId { get; set; }
        public string? Role { get; set; }
        public Transport Transport { get; set; } = default!;
        public Porter Porter { get; set; } = default!;
    }
}
