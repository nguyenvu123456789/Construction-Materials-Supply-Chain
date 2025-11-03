namespace Domain.Models
{
    public enum TransportStatus { Planned, Assigned, EnRoute, Completed, Cancelled }
    public enum TransportStopType { Depot, Pickup, Dropoff }
    public enum TransportStopStatus { Planned, Arrived, Done }

    public partial class Transport
    {
        public int TransportId { get; set; }
        public string TransportCode { get; set; } = default!;
        public TransportStatus Status { get; set; }
        public int DepotId { get; set; }
        public int ProviderPartnerId { get; set; }
        public DateTimeOffset? StartTimePlanned { get; set; }
        public DateTimeOffset? EndTimePlanned { get; set; }
        public DateTimeOffset? StartTimeActual { get; set; }
        public DateTimeOffset? EndTimeActual { get; set; }
        public string? Notes { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public virtual Address Depot { get; set; } = default!;
        public virtual Partner ProviderPartner { get; set; } = default!;
        public virtual Vehicle Vehicle { get; set; } = default!;
        public virtual Driver Driver { get; set; } = default!;
        public virtual ICollection<TransportStop> Stops { get; set; } = new List<TransportStop>();
        public virtual ICollection<TransportInvoice> TransportInvoices { get; set; } = new List<TransportInvoice>();
        public virtual ICollection<TransportPorter> TransportPorters { get; set; } = new List<TransportPorter>();
    }

    public partial class TransportPorter
    {
        public int TransportId { get; set; }
        public int PorterId { get; set; }
        public string? Role { get; set; }

        public virtual Transport Transport { get; set; } = null!;
        public virtual Porter Porter { get; set; } = null!;
    }

    public partial class TransportStop
    {
        public int TransportStopId { get; set; }
        public int TransportId { get; set; }
        public int Seq { get; set; }
        public TransportStopType StopType { get; set; }
        public int AddressId { get; set; }
        public int ServiceTimeMin { get; set; }
        public TransportStopStatus Status { get; set; }
        public DateTimeOffset? ETA { get; set; }
        public DateTimeOffset? ETD { get; set; }
        public DateTimeOffset? ATA { get; set; }
        public DateTimeOffset? ATD { get; set; }

        public virtual Transport Transport { get; set; } = null!;
        public virtual Address Address { get; set; } = null!;
    }

    public class TransportInvoice
    {
        public int TransportId { get; set; }
        public int InvoiceId { get; set; }
        public Transport Transport { get; set; } = default!;
        public Invoice Invoice { get; set; } = default!;
    }
}
