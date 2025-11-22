namespace Application.DTOs
{
    public class TransportDto
    {
        public int TransportId { get; set; }
        public string Vehicle { get; set; }
        public string Driver { get; set; }
        public string Porter { get; set; }
        public string Route { get; set; }
        public string Status { get; set; }
    }

    public class TransportCreateRequestDto
    {
        public int DepotId { get; set; }
        public int ProviderPartnerId { get; set; }
        public DateTimeOffset? StartTimePlanned { get; set; }
        public string? Notes { get; set; }
    }

    public class TransportAssignRequestDto
    {
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public List<int> PorterIds { get; set; } = new();
    }

    public class AddStopItemDto
    {
        public int Seq { get; set; }
        public string StopType { get; set; } = "Dropoff";
        public int AddressId { get; set; }
        public int ServiceTimeMin { get; set; }
    }

    public class TransportAddStopsRequestDto
    {
        public List<AddStopItemDto> Stops { get; set; } = new();
    }

    public class TransportAddInvoicesRequestDto
    {
        public List<int> InvoiceIds { get; set; } = new();
    }

    public class TransportStopArriveRequestDto
    {
        public int TransportStopId { get; set; }
        public DateTimeOffset At { get; set; }
    }

    public class TransportStopDoneRequestDto
    {
        public int TransportStopId { get; set; }
        public DateTimeOffset At { get; set; }
    }

    public class TransportStopProofBase64Dto
    {
        public int TransportStopId { get; set; }
        public string Base64 { get; set; } = default!;
    }

    public class TransportStopDto
    {
        public int TransportStopId { get; set; }
        public int Seq { get; set; }
        public string StopType { get; set; } = default!;
        public int AddressId { get; set; }
        public string AddressName { get; set; } = default!;
        public string? AddressLine1 { get; set; }
        public string? City { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public int ServiceTimeMin { get; set; }
        public string Status { get; set; } = default!;
        public DateTimeOffset? ETA { get; set; }
        public DateTimeOffset? ETD { get; set; }
        public DateTimeOffset? ATA { get; set; }
        public DateTimeOffset? ATD { get; set; }
        public List<SimpleInvoiceDto> Invoices { get; set; } = new();
    }

    public class TransportPorterDto
    {
        public int TransportId { get; set; }
        public int PorterId { get; set; }
        public string PorterName { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Role { get; set; }
    }

    public class TransportResponseDto
    {
        public int TransportId { get; set; }
        public string TransportCode { get; set; } = default!;
        public string Status { get; set; } = default!;
        public int DepotId { get; set; }
        public string DepotName { get; set; } = "";
        public int ProviderPartnerId { get; set; }
        public string ProviderPartnerName { get; set; } = "";
        public int? VehicleId { get; set; }
        public int? DriverId { get; set; }
        public DateTimeOffset? StartTimePlanned { get; set; }
        public DateTimeOffset? EndTimePlanned { get; set; }
        public DateTimeOffset? StartTimeActual { get; set; }
        public DateTimeOffset? EndTimeActual { get; set; }
        public List<TransportStopDto> Stops { get; set; } = new();
        public List<TransportPorterDto> Porters { get; set; } = new();
        public string? Notes { get; set; }
    }

    public class CustomerOrderStatusDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; } = default!;
        public DateTimeOffset IssueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string InvoiceType { get; set; } = default!;
        public string ExportStatus { get; set; } = default!;
        public string DeliveryStatus { get; set; } = default!;
    }

    public class TransportInvoiceTrackingDto
    {
        public int TransportId { get; set; }
        public string TransportCode { get; set; }
        public string Status { get; set; }

        public string DriverName { get; set; }
        public string VehicleCode { get; set; }

        public TrackingStopDto Stop { get; set; }
    }

    public class TrackingStopDto
    {
        public int StopId { get; set; }
        public string StopType { get; set; }
        public string AddressName { get; set; }
        public string City { get; set; }

        public DateTimeOffset? ETA { get; set; }
        public DateTimeOffset? ATA { get; set; }
        public DateTimeOffset? ATD { get; set; }

        public string? DeliveryPhotoBase64 { get; set; }
    }
}
