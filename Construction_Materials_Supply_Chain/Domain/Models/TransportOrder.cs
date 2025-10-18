namespace Domain.Models
{
    public class TransportOrder
    {
        public int TransportId { get; set; }
        public int OrderId { get; set; }
        public Transport Transport { get; set; } = default!;
        public Order Order { get; set; } = default!;
    }
}
