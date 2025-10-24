namespace Domain.Models
{
    public enum TransportStatus { Planned, Assigned, EnRoute, Completed, Cancelled }

    public class Transport
    {
        public int TransportId { get; set; }
        public string TransportCode { get; set; } = default!;
        public int DepotId { get; set; }
        public int? VehicleId { get; set; }
        public int? DriverId { get; set; }
        public TransportStatus Status { get; set; } = TransportStatus.Planned;
        public DateTimeOffset? StartTimePlanned { get; set; }
        public DateTimeOffset? EndTimePlanned { get; set; }
        public DateTimeOffset? StartTimeActual { get; set; }
        public DateTimeOffset? EndTimeActual { get; set; }
        public string? Notes { get; set; }
        public ICollection<TransportStop> Stops { get; set; } = new List<TransportStop>();
        public ICollection<TransportOrder> TransportOrders { get; set; } = new List<TransportOrder>();
        public ICollection<TransportPorter> TransportPorters { get; set; } = new List<TransportPorter>();

        public Vehicle? Vehicle { get; set; }
        public Driver? Driver { get; set; }
        public int ProviderPartnerId { get; set; }
        public virtual Partner ProviderPartner { get; set; } = null!;
    }
}
