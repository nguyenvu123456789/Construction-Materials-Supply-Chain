using Application.Constants.Messages;
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
                .Include(t => t.Warehouse)
                .Include(t => t.Stops)
                    .ThenInclude(s => s.TransportInvoices)
                        .ThenInclude(ti => ti.Invoice)
                .Include(t => t.TransportPorters)
                    .ThenInclude(tp => tp.Porter)
                .Include(t => t.Vehicle)
                .Include(t => t.Driver)
                .FirstOrDefault(t => t.TransportId == transportId);

        public List<Transport> Query(DateOnly? date, string? status, int? vehicleId, int? providerPartnerId = null)
        {
            var query = _context.Transports
                .Include(t => t.ProviderPartner)
                .Include(t => t.Warehouse)
                .Include(t => t.Stops)
                    .ThenInclude(s => s.TransportInvoices)
                        .ThenInclude(ti => ti.Invoice)
                .Include(t => t.TransportPorters)
                    .ThenInclude(tp => tp.Porter)
                .AsQueryable();

            if (date.HasValue)
            {
                var startOfDay = new DateTimeOffset(date.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
                var endOfDay = new DateTimeOffset(date.Value.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);

                query = query.Where(t =>
                    (t.StartTimePlanned ?? DateTimeOffset.MinValue) >= startOfDay &&
                    (t.StartTimePlanned ?? DateTimeOffset.MinValue) <= endOfDay);
            }

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TransportStatus>(status, true, out var statusEnum))
            {
                query = query.Where(t => t.Status == statusEnum);
            }

            if (vehicleId.HasValue)
            {
                query = query.Where(t => t.VehicleId == vehicleId);
            }

            if (providerPartnerId.HasValue)
            {
                query = query.Where(t => t.ProviderPartnerId == providerPartnerId);
            }

            return query.OrderByDescending(t => t.StartTimePlanned ?? DateTimeOffset.MinValue).ToList();
        }

        public void AddStops(int transportId, List<TransportStop> stops)
        {
            if (stops == null || !stops.Any()) return;

            foreach (var stop in stops)
            {
                stop.TransportId = transportId;
                _context.TransportStops.Add(stop);
            }
            _context.SaveChanges();
        }

        public void SetStopInvoices(int transportId, int transportStopId, List<int> invoiceIds)
        {
            var cleanInvoiceIds = invoiceIds?.Distinct().ToList() ?? new List<int>();

            var conflictingInvoices = _context.Set<TransportInvoice>()
                .Where(ti => cleanInvoiceIds.Contains(ti.InvoiceId) && ti.TransportStopId != transportStopId)
                .Select(ti => ti.InvoiceId)
                .ToList();

            if (conflictingInvoices.Any())
            {
                var conflictList = string.Join(", ", conflictingInvoices);
                throw new InvalidOperationException(string.Format(TransportMessages.INVOICE_ASSIGNED_ELSEWHERE, conflictList));
            }

            var dbSet = _context.Set<TransportInvoice>();
            var currentInvoices = dbSet.Where(ti => ti.TransportStopId == transportStopId).ToList();
            var newInvoiceSet = cleanInvoiceIds.ToHashSet();

            var toRemove = currentInvoices.Where(ti => !newInvoiceSet.Contains(ti.InvoiceId)).ToList();
            if (toRemove.Any())
            {
                _context.RemoveRange(toRemove);
            }

            var existingInvoiceIds = currentInvoices.Select(ti => ti.InvoiceId).ToHashSet();
            foreach (var id in cleanInvoiceIds)
            {
                if (!existingInvoiceIds.Contains(id))
                {
                    dbSet.Add(new TransportInvoice { TransportStopId = transportStopId, InvoiceId = id });
                }
            }

            _context.SaveChanges();
        }

        public bool InvoiceAssignedElsewhere(IEnumerable<int> invoiceIds, int transportId, int transportStopId)
        {
            if (invoiceIds == null || !invoiceIds.Any()) return false;
            return _context.Set<TransportInvoice>()
                .Any(ti => invoiceIds.Contains(ti.InvoiceId) && ti.TransportStopId != transportStopId);
        }

        public void UpdateStatus(int transportId, TransportStatus status, DateTimeOffset? startActual, DateTimeOffset? endActual)
        {
            var transport = _context.Transports.First(x => x.TransportId == transportId);
            transport.Status = status;

            if (startActual.HasValue) transport.StartTimeActual = startActual;
            if (endActual.HasValue) transport.EndTimeActual = endActual;

            _context.SaveChanges();
        }

        public void UpdateStopArrival(int transportId, int transportStopId, DateTimeOffset at)
        {
            var stop = _context.TransportStops.First(x => x.TransportId == transportId && x.TransportStopId == transportStopId);
            stop.ATA = at;
            stop.Status = TransportStopStatus.Arrived;
            _context.SaveChanges();
        }

        public void UpdateStopDeparture(int transportId, int transportStopId, DateTimeOffset at)
        {
            var stop = _context.TransportStops.First(x => x.TransportId == transportId && x.TransportStopId == transportStopId);

            if (string.IsNullOrEmpty(stop.ProofImageBase64))
                throw new InvalidOperationException(TransportMessages.STOP_PROOF_REQUIRED);

            stop.ATD = at;
            stop.Status = TransportStopStatus.Done;
            _context.SaveChanges();
        }

        public void UpdateStopProofImage(int transportId, int transportStopId, string proofImageBase64)
        {
            var stop = _context.TransportStops.First(x => x.TransportId == transportId && x.TransportStopId == transportStopId);
            stop.ProofImageBase64 = proofImageBase64;
            _context.SaveChanges();
        }

        public bool AllNonDepotStopsDone(int transportId)
        {
            return !_context.TransportStops.Any(s =>
                s.TransportId == transportId &&
                s.StopType != TransportStopType.Depot &&
                s.Status != TransportStopStatus.Done);
        }

        public void Cancel(int transportId, string reason)
        {
            var transport = _context.Transports.First(x => x.TransportId == transportId);
            transport.Status = TransportStatus.Cancelled;

            var prefix = string.IsNullOrEmpty(transport.Notes) ? "" : transport.Notes + " | ";
            transport.Notes = $"{prefix}Cancelled:{reason}";

            _context.SaveChanges();
        }

        private void ResequenceStops(int transportId)
        {
            var stops = _context.TransportStops
                .Where(x => x.TransportId == transportId)
                .OrderBy(x => x.Seq)
                .ToList();

            for (int i = 0; i < stops.Count; i++)
            {
                stops[i].Seq = i;
            }
            _context.SaveChanges();
        }

        public void RemoveStop(int transportId, int transportStopId)
        {
            var stop = _context.TransportStops.First(x => x.TransportId == transportId && x.TransportStopId == transportStopId);

            if (stop.Seq == 0)
                throw new InvalidOperationException(TransportMessages.STOP_CANNOT_REMOVE_DEPOT);

            _context.TransportStops.Remove(stop);
            _context.SaveChanges();
            ResequenceStops(transportId);
        }

        public void ClearStops(int transportId, bool keepDepot)
        {
            var query = _context.TransportStops.Where(x => x.TransportId == transportId);

            if (keepDepot)
                query = query.Where(x => x.Seq != 0);

            _context.TransportStops.RemoveRange(query);
            _context.SaveChanges();
            ResequenceStops(transportId);
        }

        public bool VehicleBusy(int vehicleId, int excludeTransportId, DateTimeOffset start, DateTimeOffset? end)
        {
            var endTime = end ?? DateTimeOffset.MaxValue;
            return _context.Transports.Any(t =>
                t.TransportId != excludeTransportId &&
                t.VehicleId == vehicleId &&
                ActiveStatuses.Contains(t.Status) &&
                (t.StartTimeActual ?? t.StartTimePlanned) < endTime &&
                (t.EndTimeActual ?? t.EndTimePlanned ?? DateTimeOffset.MaxValue) > start);
        }

        public bool DriverBusy(int driverId, int excludeTransportId, DateTimeOffset start, DateTimeOffset? end)
        {
            var endTime = end ?? DateTimeOffset.MaxValue;
            return _context.Transports.Any(t =>
                t.TransportId != excludeTransportId &&
                t.DriverId == driverId &&
                ActiveStatuses.Contains(t.Status) &&
                (t.StartTimeActual ?? t.StartTimePlanned) < endTime &&
                (t.EndTimeActual ?? t.EndTimePlanned ?? DateTimeOffset.MaxValue) > start);
        }

        public List<int> BusyPorters(List<int> porterIds, int excludeTransportId, DateTimeOffset start, DateTimeOffset? end)
        {
            var endTime = end ?? DateTimeOffset.MaxValue;
            return _context.TransportPorters
                .Where(tp =>
                    porterIds.Contains(tp.PorterId) &&
                    tp.TransportId != excludeTransportId &&
                    ActiveStatuses.Contains(tp.Transport.Status) &&
                    (tp.Transport.StartTimeActual ?? tp.Transport.StartTimePlanned) < endTime &&
                    (tp.Transport.EndTimeActual ?? tp.Transport.EndTimePlanned ?? DateTimeOffset.MaxValue) > start
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

        private static (DateTimeOffset start, DateTimeOffset end) GetWindow(Transport t)
        {
            var start = t.StartTimeActual ?? t.StartTimePlanned ?? DateTimeOffset.MinValue;
            var end = t.EndTimeActual ?? t.EndTimePlanned ?? DateTimeOffset.MaxValue;
            return (start, end);
        }

        public DateTimeOffset? VehicleBusyUntil(int vehicleId, DateTimeOffset start, DateTimeOffset end)
        {
            var query = _context.Transports
                .Where(t => t.VehicleId == vehicleId && ActiveStatuses.Contains(t.Status));

            var hit = query.AsEnumerable()
                       .Select(GetWindow)
                       .FirstOrDefault(w => w.start < end && w.end > start);

            return hit == default ? null : hit.end;
        }

        public DateTimeOffset? DriverBusyUntil(int driverId, DateTimeOffset start, DateTimeOffset end)
        {
            var query = _context.Transports
                .Where(t => t.DriverId == driverId && ActiveStatuses.Contains(t.Status));

            var hit = query.AsEnumerable()
                       .Select(GetWindow)
                       .FirstOrDefault(w => w.start < end && w.end > start);

            return hit == default ? null : hit.end;
        }

        public DateTimeOffset? PorterBusyUntil(int porterId, DateTimeOffset start, DateTimeOffset end)
        {
            var query = _context.TransportPorters
                .Where(tp => tp.PorterId == porterId && ActiveStatuses.Contains(tp.Transport.Status))
                .Select(tp => tp.Transport);

            var hit = query.AsEnumerable()
                       .Select(GetWindow)
                       .FirstOrDefault(w => w.start < end && w.end > start);

            return hit == default ? null : hit.end;
        }

        public void ReplacePorters(int transportId, List<int> porterIds)
        {
            var transport = _context.Transports
                .Include(x => x.TransportPorters)
                .First(x => x.TransportId == transportId);

            _context.RemoveRange(transport.TransportPorters);

            if (porterIds != null)
            {
                foreach (var porterId in porterIds)
                {
                    _context.TransportPorters.Add(new TransportPorter
                    {
                        TransportId = transportId,
                        PorterId = porterId,
                        Role = "Member"
                    });
                }
            }
            _context.SaveChanges();
        }

        public List<Transport> GetByInvoiceId(int invoiceId)
        {
            return _context.Transports
                .Include(t => t.ProviderPartner)
                .Include(t => t.Warehouse)
                .Include(t => t.Stops)
                    .ThenInclude(s => s.TransportInvoices)
                        .ThenInclude(ti => ti.Invoice)
                .Include(t => t.TransportPorters)
                    .ThenInclude(tp => tp.Porter)
                .Include(t => t.Vehicle)
                .Include(t => t.Driver)
                .Where(t => t.Stops.Any(s => s.TransportInvoices.Any(ti => ti.InvoiceId == invoiceId)))
                .ToList();
        }

        public TransportStop? GetStopByInvoice(int invoiceId)
        {
            return _context.TransportStops
                .Include(s => s.Transport).ThenInclude(t => t.Vehicle)
                .Include(s => s.Transport).ThenInclude(t => t.Driver)
                .Include(s => s.TransportInvoices)
                .FirstOrDefault(s => s.TransportInvoices.Any(ti => ti.InvoiceId == invoiceId));
        }
    }
}