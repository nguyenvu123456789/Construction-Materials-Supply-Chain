using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface ITransportRepository : IGenericRepository<Transport>
    {
        Transport? GetDetail(int transportId);
        List<Transport> Query(DateOnly? date, string? status, int? vehicleId, int? providerPartnerId = null);

        void AddStops(int transportId, List<TransportStop> stops);
        void AddOrders(int transportId, List<TransportOrder> orders);

        void UpdateStatus(int transportId, TransportStatus status, DateTimeOffset? startActual, DateTimeOffset? endActual);
        void UpdateStopArrival(int transportId, int transportStopId, DateTimeOffset at);
        void UpdateStopDeparture(int transportId, int transportStopId, DateTimeOffset at);
        bool AllNonDepotStopsDone(int transportId);
        void Cancel(int transportId, string reason);
        void RemoveStop(int transportId, int transportStopId);
        void ClearStops(int transportId, bool keepDepot);

        bool VehicleBusy(int vehicleId, int excludeTransportId, DateTimeOffset s, DateTimeOffset? e);
        bool DriverBusy(int driverId, int excludeTransportId, DateTimeOffset s, DateTimeOffset? e);
        List<int> BusyPorters(List<int> porterIds, int excludeTransportId, DateTimeOffset s, DateTimeOffset? e);

        List<int> GetPorterIds(int transportId);
        int? GetVehicleId(int transportId);
        int? GetDriverId(int transportId);

        DateTimeOffset? VehicleBusyUntil(int vehicleId, DateTimeOffset s, DateTimeOffset e);
        DateTimeOffset? DriverBusyUntil(int driverId, DateTimeOffset s, DateTimeOffset e);
        DateTimeOffset? PorterBusyUntil(int porterId, DateTimeOffset s, DateTimeOffset e);

        void ReplacePorters(int transportId, List<int> porterIds);
    }
}
