using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class TransportRepository : GenericRepository<Transport>, ITransportRepository
    {
        public TransportRepository(ScmVlxdContext context) : base(context) { }

        public Transport? GetDetail(int transportId) =>
            _context.Transports
                .Include(t => t.Stops).ThenInclude(s => s.Address)
                .Include(t => t.TransportOrders).ThenInclude(to => to.Order)
                .Include(t => t.TransportPorters).ThenInclude(tp => tp.Porter)
                .Include(t => t.Vehicle)
                .Include(t => t.Driver)
                .FirstOrDefault(t => t.TransportId == transportId);

        public List<Transport> Query(DateOnly? date, string? status, int? vehicleId)
        {
            var q = _context.Transports
                .Include(t => t.Stops).ThenInclude(s => s.Address)
                .Include(t => t.TransportOrders).ThenInclude(to => to.Order)
                .Include(t => t.TransportPorters).ThenInclude(tp => tp.Porter)
                .Include(t => t.Vehicle)
                .Include(t => t.Driver)
                .AsQueryable();
            if (date is not null)
            {
                var d0 = date.Value.ToDateTime(TimeOnly.MinValue);
                var d1 = date.Value.ToDateTime(TimeOnly.MaxValue);
                q = q.Where(t => t.StartTimePlanned >= d0 && t.StartTimePlanned <= d1);
            }
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TransportStatus>(status, true, out var st))
                q = q.Where(t => t.Status == st);
            if (vehicleId is not null)
                q = q.Where(t => t.VehicleId == vehicleId);
            return q.OrderByDescending(t => t.StartTimePlanned ?? DateTimeOffset.MinValue).ToList();
        }

        public void Assign(int transportId, int vehicleId, int driverId, List<int> porterIds)
        {
            var t = _context.Transports.Include(x => x.TransportPorters).First(x => x.TransportId == transportId);
            t.VehicleId = vehicleId;
            t.DriverId = driverId;
            t.TransportPorters.Clear();
            foreach (var p in porterIds) t.TransportPorters.Add(new TransportPorter { TransportId = transportId, PorterId = p, Role = "Member" });
            t.Status = TransportStatus.Assigned;
            _context.SaveChanges();
        }

        public void AddStops(int transportId, List<TransportStop> stops)
        {
            foreach (var s in stops) { s.TransportId = transportId; _context.TransportStops.Add(s); }
            _context.SaveChanges();
        }

        public void AddOrders(int transportId, List<TransportOrder> orders)
        {
            foreach (var o in orders)
            {
                var exists = _context.TransportOrders.Any(x => x.TransportId == transportId && x.OrderId == o.OrderId);
                if (!exists) { o.TransportId = transportId; _context.TransportOrders.Add(o); }
            }
            _context.SaveChanges();
        }

        public void UpdateStatus(int transportId, TransportStatus status, DateTimeOffset? startActual, DateTimeOffset? endActual)
        {
            var t = _context.Transports.First(x => x.TransportId == transportId);
            t.Status = status;
            if (startActual.HasValue) t.StartTimeActual = startActual;
            if (endActual.HasValue) t.EndTimeActual = endActual;
            _context.SaveChanges();
        }

        public void UpdateStopArrival(int transportId, int transportStopId, DateTimeOffset at)
        {
            var s = _context.TransportStops.First(x => x.TransportId == transportId && x.TransportStopId == transportStopId);
            s.ATA = at;
            s.Status = TransportStopStatus.Arrived;
            _context.SaveChanges();
        }

        public void UpdateStopDeparture(int transportId, int transportStopId, DateTimeOffset at)
        {
            var s = _context.TransportStops.First(x => x.TransportId == transportId && x.TransportStopId == transportStopId);
            s.ATD = at;
            s.Status = TransportStopStatus.Done;
            _context.SaveChanges();
        }

        public bool AllNonDepotStopsDone(int transportId) =>
            !_context.TransportStops.Any(s => s.TransportId == transportId && s.StopType != TransportStopType.Depot && s.Status != TransportStopStatus.Done);

        public void Cancel(int transportId, string reason)
        {
            var t = _context.Transports.First(x => x.TransportId == transportId);
            t.Status = TransportStatus.Cancelled;
            t.Notes = (t.Notes is null ? "" : t.Notes + " | ") + $"Cancelled:{reason}";
            _context.SaveChanges();
        }
    }
}
