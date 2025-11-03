using Application.DTOs;

namespace Application.Interfaces
{
    public interface ITransportService
    {
        TransportResponseDto Create(TransportCreateRequestDto dto);
        TransportResponseDto? Get(int transportId);
        List<TransportResponseDto> Query(DateOnly? date, string? status, int? vehicleId, int? providerPartnerId = null);
        void Assign(int transportId, TransportAssignRequestDto dto);
        void AddStops(int transportId, TransportAddStopsRequestDto dto);
        void AddOrders(int transportId, TransportAddOrdersRequestDto dto);
        void Start(int transportId, DateTimeOffset at);
        void ArriveStop(int transportId, TransportStopArriveRequestDto dto);
        void CompleteStop(int transportId, TransportStopDoneRequestDto dto);
        void Complete(int transportId, DateTimeOffset at);
        void Cancel(int transportId, string reason);
        void DeleteStop(int transportId, int transportStopId);
        void ClearStops(int transportId, bool keepDepot);
    }
}
