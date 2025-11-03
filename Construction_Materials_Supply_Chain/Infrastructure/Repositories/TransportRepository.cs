using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class TransportRepository : GenericRepository<Transport>, ITransportRepository
    {
        private static readonly TransportStatus[] ActiveStatuses =
        {
            TransportStatus.Planned,
            TransportStatus.Assigned,
            TransportStatus.EnRoute
        };

        public TransportRepository(ScmVlxdContext context) : base(context) { }

        public Transport? GetDetail(int transportId) =>
            _context.Transports
                .Include(t => t.ProviderPartner)
                .Include(t => t.Depot)
                .Include(t => t.Stops).ThenInclude(s => s.Address)
                .Include(t => t.TransportOrders).ThenInclude(to => to.Order)
                .Include(t => t.TransportPorters).ThenInclude(tp => tp.Porter)
                .Include(t => t.Vehicle)
                .Include(t => t.Driver)
                .FirstOrDefault(t => t.TransportId == transportId);

        public List<Transport> Query(DateOnly? date, string? status, int? vehicleId, int? providerPartnerId = null)
        {
            var q = _context.Transports
                .Include(t => t.ProviderPartner)
                .Include(t => t.Depot)
                .Include(t => t.Stops).ThenInclude(s => s.Address)
                .Include(t => t.TransportOrders).ThenInclude(to => to.Order)
                .Include(t => t.TransportPorters).ThenInclude(tp => tp.Porter)
                .AsQueryable();

            if (date is not null)
            {
                var d0 = new DateTimeOffset(date.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
                var d1 = new DateTimeOffset(date.Value.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);
                q = q.Where(t =>
                    (t.StartTimePlanned ?? DateTimeOffset.MinValue) >= d0 &&
                    (t.StartTimePlanned ?? DateTimeOffset.MinValue) <= d1);
            }

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TransportStatus>(status, true, out var st))
                q = q.Where(t => t.Status == st);

            if (vehicleId is not null)
                q = q.Where(t => t.VehicleId == vehicleId);

            if (providerPartnerId is not null)
                q = q.Where(t => t.ProviderPartnerId == providerPartnerId);

            return q.OrderByDescending(t => t.StartTimePlanned ?? DateTimeOffset.MinValue).ToList();
        }

        public void AddStops(int transportId, List<TransportStop> stops)
        {
            foreach (var s in stops)
            {
                s.TransportId = transportId;
                _context.TransportStops.Add(s);
            }
            _context.SaveChanges();
        }

        public void AddOrders(int transportId, List<TransportOrder> orders)
        {
            foreach (var o in orders)
            {
                var exists = _context.TransportOrders.Any(x => x.TransportId == transportId && x.OrderId == o.OrderId);
                if (!exists)
                {
                    o.TransportId = transportId;
                    _context.TransportOrders.Add(o);
                }
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
            !_context.TransportStops.Any(s => s.TransportId == transportId
                                           && s.StopType != TransportStopType.Depot
                                           && s.Status != TransportStopStatus.Done);

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
            return _context.Transports.Any(t =>
                t.TransportId != excludeTransportId &&
                t.VehicleId == vehicleId &&
                System.Linq.Enumerable.Contains(ActiveStatuses, t.Status) &&
                (t.StartTimeActual ?? t.StartTimePlanned) < e1 &&
                (t.EndTimeActual ?? t.EndTimePlanned ?? DateTimeOffset.MaxValue) > s);
        }

        public bool DriverBusy(int driverId, int excludeTransportId, DateTimeOffset s, DateTimeOffset? e)
        {
            var e1 = e ?? DateTimeOffset.MaxValue;
            return _context.Transports.Any(t =>
                t.TransportId != excludeTransportId &&
                t.DriverId == driverId &&
                System.Linq.Enumerable.Contains(ActiveStatuses, t.Status) &&
                (t.StartTimeActual ?? t.StartTimePlanned) < e1 &&
                (t.EndTimeActual ?? t.EndTimePlanned ?? DateTimeOffset.MaxValue) > s);
        }

        public List<int> BusyPorters(List<int> porterIds, int excludeTransportId, DateTimeOffset s, DateTimeOffset? e)
        {
            var e1 = e ?? DateTimeOffset.MaxValue;
            return _context.TransportPorters
                .Where(tp =>
                    porterIds.Contains(tp.PorterId) &&
                    tp.TransportId != excludeTransportId &&
                    System.Linq.Enumerable.Contains(ActiveStatuses, tp.Transport.Status) &&
                    (tp.Transport.StartTimeActual ?? tp.Transport.StartTimePlanned) < e1 &&
                    (tp.Transport.EndTimeActual ?? tp.Transport.EndTimePlanned ?? DateTimeOffset.MaxValue) > s
                )
                .Select(tp => tp.PorterId)
                .Distinct()
                .ToList();
        }

        public List<int> GetPorterIds(int transportId) =>
            _context.TransportPorters
                .Where(tp => tp.TransportId == transportId)
                .Select(tp => tp.PorterId)
                .ToList();

        public int? GetVehicleId(int transportId) =>
            _context.Transports
                .Where(t => t.TransportId == transportId)
                .Select(t => (int?)t.VehicleId)
                .FirstOrDefault();

        public int? GetDriverId(int transportId) =>
            _context.Transports
                .Where(t => t.TransportId == transportId)
                .Select(t => (int?)t.DriverId)
                .FirstOrDefault();

        private static (DateTimeOffset start, DateTimeOffset end) WindowOf(Transport t)
        {
            var s = t.StartTimeActual ?? t.StartTimePlanned ?? DateTimeOffset.MinValue;
            var e = t.EndTimeActual ?? t.EndTimePlanned ?? DateTimeOffset.MaxValue;
            return (s, e);
        }

        public DateTimeOffset? VehicleBusyUntil(int vehicleId, DateTimeOffset s, DateTimeOffset e)
        {
            var q = _context.Transports
                .Where(t => t.VehicleId == vehicleId
                         && System.Linq.Enumerable.Contains(ActiveStatuses, t.Status));

            var hit = q.AsEnumerable()
                       .Select(WindowOf)
                       .FirstOrDefault(w => w.start < e && w.end > s);
            return hit == default ? null : hit.end;
        }

        public DateTimeOffset? DriverBusyUntil(int driverId, DateTimeOffset s, DateTimeOffset e)
        {
            var q = _context.Transports
                .Where(t => t.DriverId == driverId
                         && System.Linq.Enumerable.Contains(ActiveStatuses, t.Status));

            var hit = q.AsEnumerable()
                       .Select(WindowOf)
                       .FirstOrDefault(w => w.start < e && w.end > s);
            return hit == default ? null : hit.end;
        }

        public DateTimeOffset? PorterBusyUntil(int porterId, DateTimeOffset s, DateTimeOffset e)
        {
            var q = _context.TransportPorters
                .Where(tp => tp.PorterId == porterId
                          && System.Linq.Enumerable.Contains(ActiveStatuses, tp.Transport.Status))
                .Select(tp => tp.Transport);

            var hit = q.AsEnumerable()
                       .Select(WindowOf)
                       .FirstOrDefault(w => w.start < e && w.end > s);
            return hit == default ? null : hit.end;
        }

        public void ReplacePorters(int transportId, List<int> porterIds)
        {
            var t = _context.Transports
                .Include(x => x.TransportPorters)
                .First(x => x.TransportId == transportId);

            _context.RemoveRange(t.TransportPorters);

            foreach (var p in porterIds)
                _context.TransportPorters.Add(new TransportPorter
                {
                    TransportId = transportId,
                    PorterId = p,
                    Role = "Member"
                });

            _context.SaveChanges();
        }
    }
}
