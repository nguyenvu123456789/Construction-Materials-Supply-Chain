namespace Domain.Models
{
    public class ShippingLog
    {
        public int ShippingLogId { get; set; }
        public int TransportId { get; set; }
        public int? TransportStopId { get; set; }
        public int? InvoiceId { get; set; }
        public string Status { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; }

        public Transport Transport { get; set; } = default!;
        public TransportStop? TransportStop { get; set; }
        public Invoice? Invoice { get; set; }
    }
}
