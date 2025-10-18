namespace Domain.Models
{
    public enum TransportStopType { Depot, Pickup, Dropoff }
    public enum TransportStopStatus { Planned, Arrived, Servicing, Done }

    public class TransportStop
    {
        public int TransportStopId { get; set; }
        public int TransportId { get; set; }
        public int Seq { get; set; }
        public TransportStopType StopType { get; set; }
        public int AddressId { get; set; }
        public int ServiceTimeMin { get; set; }
        public TransportStopStatus Status { get; set; } = TransportStopStatus.Planned;
        public DateTimeOffset? ETA { get; set; }
        public DateTimeOffset? ETD { get; set; }
        public DateTimeOffset? ATA { get; set; }
        public DateTimeOffset? ATD { get; set; }
        public Transport Transport { get; set; } = default!;
        public Address Address { get; set; } = default!;
    }
}
