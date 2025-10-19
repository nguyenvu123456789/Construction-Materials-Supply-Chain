using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface ITransportRepository : IGenericRepository<Transport>
    {
        Transport? GetDetail(int transportId);
        List<Transport> Query(DateOnly? date, string? status, int? vehicleId);
        void Assign(int transportId, int vehicleId, int driverId, List<int> porterIds);
        void AddStops(int transportId, List<TransportStop> stops);
        void AddOrders(int transportId, List<TransportOrder> orders);
        void UpdateStatus(int transportId, TransportStatus status, DateTimeOffset? startActual, DateTimeOffset? endActual);
        void UpdateStopArrival(int transportId, int transportStopId, DateTimeOffset at);
        void UpdateStopDeparture(int transportId, int transportStopId, DateTimeOffset at);
        bool AllNonDepotStopsDone(int transportId);
        void Cancel(int transportId, string reason);
        void RemoveStop(int transportId, int transportStopId);
        void ClearStops(int transportId, bool keepDepot);
    }
}
