using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class TransportRepository : GenericRepository<Transport>, ITransportRepository
    {
        private static readonly string[] ActiveStatuses = new[] { "Planned", "Assigned", "EnRoute" };

        public TransportRepository(ScmVlxdContext context) : base(context) { }

        public Transport? GetDetail(int transportId) =>
            _context.Transports
                .Include(t => t.ProviderPartner)
                .Include(t => t.Depot)
                .Include(t => t.Stops).ThenInclude(s => s.Address)
                .Include(t => t.TransportOrders).ThenInclude(to => to.Order)
                .Include(t => t.TransportPorters).ThenInclude(tp => tp.Porter)
                .Include(t => t.Assignments).ThenInclude(a => a.Vehicle)
                .Include(t => t.Assignments).ThenInclude(a => a.Driver)
                .FirstOrDefault(t => t.TransportId == transportId);

        public List<Transport> Query(DateOnly? date, string? status, int? vehicleId, int? providerPartnerId = null)
        {
            var q = _context.Transports
                .Include(t => t.ProviderPartner)
                .Include(t => t.Depot)
                .Include(t => t.Stops).ThenInclude(s => s.Address)
                .Include(t => t.TransportOrders).ThenInclude(to => to.Order)
                .Include(t => t.TransportPorters).ThenInclude(tp => tp.Porter)
                .Include(t => t.Assignments).ThenInclude(a => a.Vehicle)
                .Include(t => t.Assignments).ThenInclude(a => a.Driver)
                .AsQueryable();

            if (date is not null)
            {
                var d0 = date.Value.ToDateTime(TimeOnly.MinValue);
                var d1 = date.Value.ToDateTime(TimeOnly.MaxValue);
                q = q.Where(t => (t.StartTimePlanned ?? DateTimeOffset.MinValue) >= d0
                              && (t.StartTimePlanned ?? DateTimeOffset.MinValue) <= d1);
            }

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TransportStatus>(status, true, out var st))
                q = q.Where(t => t.Status == st);

            if (vehicleId is not null)
                q = q.Where(t => t.Assignments.Any(a => a.VehicleId == vehicleId));

            if (providerPartnerId is not null)
                q = q.Where(t => t.ProviderPartnerId == providerPartnerId);

            return q.OrderByDescending(t => t.StartTimePlanned ?? DateTimeOffset.MinValue).ToList();
        }

        public void AssignMulti(int transportId, List<TransportAssignment> assigns, List<int> porterIds)
        {
            var t = _context.Transports
                .Include(x => x.Assignments)
                .Include(x => x.TransportPorters)
                .First(x => x.TransportId == transportId);

            _context.RemoveRange(t.Assignments);
            t.Assignments.Clear();
            foreach (var a in assigns)
            {
                a.TransportId = transportId;
                _context.Set<TransportAssignment>().Add(a);
            }

            _context.RemoveRange(t.TransportPorters);
            t.TransportPorters.Clear();
            foreach (var p in porterIds)
                _context.Set<TransportPorter>().Add(new TransportPorter { TransportId = transportId, PorterId = p, Role = "Member" });

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

        private void ResequenceStops(int transportId)
        {
            var stops = _context.TransportStops.Where(x => x.TransportId == transportId).OrderBy(x => x.Seq).ToList();
            var i = 0;
            foreach (var s in stops) s.Seq = i++;
            _context.SaveChanges();
        }

        public void RemoveStop(int transportId, int transportStopId)
        {
            var s = _context.TransportStops.First(x => x.TransportId == transportId && x.TransportStopId == transportStopId);
            if (s.Seq == 0) throw new InvalidOperationException("Cannot remove depot stop");
            _context.TransportStops.Remove(s);
            _context.SaveChanges();
            ResequenceStops(transportId);
        }

        public void ClearStops(int transportId, bool keepDepot)
        {
            var q = _context.TransportStops.Where(x => x.TransportId == transportId);
            if (keepDepot) q = q.Where(x => x.Seq != 0);
            _context.TransportStops.RemoveRange(q);
            _context.SaveChanges();
            ResequenceStops(transportId);
        }

        public bool VehicleBusy(int vehicleId, int excludeTransportId, DateTimeOffset s, DateTimeOffset? e)
        {
            var e1 = e ?? DateTimeOffset.MaxValue;
            return _context.Set<TransportAssignment>()
                .Include(a => a.Transport)
                .Any(a =>
                    a.TransportId != excludeTransportId &&
                    a.VehicleId == vehicleId &&
                    ActiveStatuses.Contains(a.Transport.Status.ToString()) &&
                    (a.Transport.StartTimeActual ?? a.Transport.StartTimePlanned) < e1 &&
                    (a.Transport.EndTimeActual ?? a.Transport.EndTimePlanned ?? DateTimeOffset.MaxValue) > s
                );
        }

        public bool DriverBusy(int driverId, int excludeTransportId, DateTimeOffset s, DateTimeOffset? e)
        {
            var e1 = e ?? DateTimeOffset.MaxValue;
            return _context.Set<TransportAssignment>()
                .Include(a => a.Transport)
                .Any(a =>
                    a.TransportId != excludeTransportId &&
                    a.DriverId == driverId &&
                    ActiveStatuses.Contains(a.Transport.Status.ToString()) &&
                    (a.Transport.StartTimeActual ?? a.Transport.StartTimePlanned) < e1 &&
                    (a.Transport.EndTimeActual ?? a.Transport.EndTimePlanned ?? DateTimeOffset.MaxValue) > s
                );
        }

        public List<int> BusyPorters(List<int> porterIds, int excludeTransportId, DateTimeOffset s, DateTimeOffset? e)
        {
            var e1 = e ?? DateTimeOffset.MaxValue;
            return _context.TransportPorters
                .Where(tp =>
                    porterIds.Contains(tp.PorterId) &&
                    tp.TransportId != excludeTransportId &&
                    ActiveStatuses.Contains(tp.Transport.Status.ToString()) &&
                    (tp.Transport.StartTimeActual ?? tp.Transport.StartTimePlanned) < e1 &&
                    (tp.Transport.EndTimeActual ?? tp.Transport.EndTimePlanned ?? DateTimeOffset.MaxValue) > s
                )
                .Select(tp => tp.PorterId)
                .Distinct()
                .ToList();
        }

        public List<int> GetPorterIds(int transportId) =>
            _context.TransportPorters.Where(tp => tp.TransportId == transportId).Select(tp => tp.PorterId).ToList();

        public List<int> GetVehicleIds(int transportId) =>
            _context.Set<TransportAssignment>().Where(a => a.TransportId == transportId).Select(a => a.VehicleId).Distinct().ToList();

        public List<int> GetDriverIds(int transportId) =>
            _context.Set<TransportAssignment>().Where(a => a.TransportId == transportId).Select(a => a.DriverId).Distinct().ToList();

        private static (DateTimeOffset start, DateTimeOffset end) WindowOf(Transport t)
        {
            var s = t.StartTimeActual ?? t.StartTimePlanned ?? DateTimeOffset.MinValue;
            var e = t.EndTimeActual ?? t.EndTimePlanned ?? DateTimeOffset.MaxValue;
            return (s, e);
        }

        public DateTimeOffset? VehicleBusyUntil(int vehicleId, DateTimeOffset s, DateTimeOffset e)
        {
            var q = _context.Set<TransportAssignment>()
                .Include(a => a.Transport)
                .Where(a => a.VehicleId == vehicleId && ActiveStatuses.Contains(a.Transport.Status.ToString()))
                .Select(a => a.Transport);
            var hit = q.AsEnumerable().Select(WindowOf).FirstOrDefault(w => w.start < e && w.end > s);
            return hit == default ? null : hit.end;
        }

        public DateTimeOffset? DriverBusyUntil(int driverId, DateTimeOffset s, DateTimeOffset e)
        {
            var q = _context.Set<TransportAssignment>()
                .Include(a => a.Transport)
                .Where(a => a.DriverId == driverId && ActiveStatuses.Contains(a.Transport.Status.ToString()))
                .Select(a => a.Transport);
            var hit = q.AsEnumerable().Select(WindowOf).FirstOrDefault(w => w.start < e && w.end > s);
            return hit == default ? null : hit.end;
        }

        public DateTimeOffset? PorterBusyUntil(int porterId, DateTimeOffset s, DateTimeOffset e)
        {
            var q = _context.TransportPorters
                .Where(tp => tp.PorterId == porterId && ActiveStatuses.Contains(tp.Transport.Status.ToString()))
                .Select(tp => tp.Transport);
            var hit = q.AsEnumerable().Select(WindowOf).FirstOrDefault(w => w.start < e && w.end > s);
            return hit == default ? null : hit.end;
        }
    }
}
