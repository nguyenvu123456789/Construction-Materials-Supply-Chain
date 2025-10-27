﻿namespace Application.DTOs
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

    public class TransportAssignItemDto
    {
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public DateTimeOffset? StartTimePlanned { get; set; }
        public DateTimeOffset? EndTimePlanned { get; set; }
    }

    public class TransportAssignMultiRequestDto
    {
        public List<TransportAssignItemDto> Assignments { get; set; } = new();
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

    public class TransportAddOrdersRequestDto
    {
        public List<int> OrderIds { get; set; } = new();
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

    public class TransportAssignmentDto
    {
        public int TransportAssignmentId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleCode { get; set; } = default!;
        public string VehiclePlate { get; set; } = default!;
        public int DriverId { get; set; }
        public string DriverName { get; set; } = default!;
        public string? DriverPhone { get; set; }
        public DateTimeOffset? StartTimePlanned { get; set; }
        public DateTimeOffset? EndTimePlanned { get; set; }
        public DateTimeOffset? StartTimeActual { get; set; }
        public DateTimeOffset? EndTimeActual { get; set; }
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
    }

    public class TransportOrderDto
    {
        public int TransportId { get; set; }
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = default!;
        public string? CustomerName { get; set; }
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
        public DateTimeOffset? StartTimePlanned { get; set; }
        public DateTimeOffset? EndTimePlanned { get; set; }
        public DateTimeOffset? StartTimeActual { get; set; }
        public DateTimeOffset? EndTimeActual { get; set; }
        public List<TransportAssignmentDto> Assignments { get; set; } = new();
        public List<TransportStopDto> Stops { get; set; } = new();
        public List<TransportOrderDto> Orders { get; set; } = new();
        public List<TransportPorterDto> Porters { get; set; } = new();
        public string? Notes { get; set; }
    }
}




