using Application.DTOs;
using Application.DTOs.Common;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class TransportService : ITransportService
    {
        private readonly ITransportRepository _transportRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IShippingLogRepository _logRepo;
        private readonly IDriverRepository _driverRepo;
        private readonly IPorterRepository _porterRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IPartnerRepository _partnerRepo;
        private readonly IMapper _mapper;
        private readonly IEventNotificationService _event;

        public TransportService(
            ITransportRepository transportRepo,
            IInvoiceRepository invoiceRepo,
            IShippingLogRepository logRepo,
            IDriverRepository driverRepo,
            IPorterRepository porterRepo,
            IVehicleRepository vehicleRepo,
            IPartnerRepository partnerRepo,
            IMapper mapper,
            IEventNotificationService eventSvc)
        {
            _transportRepo = transportRepo;
            _invoiceRepo = invoiceRepo;
            _logRepo = logRepo;
            _driverRepo = driverRepo;
            _porterRepo = porterRepo;
            _vehicleRepo = vehicleRepo;
            _partnerRepo = partnerRepo;
            _mapper = mapper;
            _event = eventSvc;
        }

        private static readonly string[] LicenseOrder = new[]
        {
            "A1","A","B1","B","C1","C","D1","D2","D","BE","C1E","CE","D1E","D2E","DE"
        };

        private static int LicenseRank(string? cls)
        {
            if (string.IsNullOrWhiteSpace(cls)) return -1;
            var x = cls.Trim().ToUpperInvariant();
            for (int i = 0; i < LicenseOrder.Length; i++)
                if (LicenseOrder[i].Equals(x, StringComparison.OrdinalIgnoreCase))
                    return i;
            return -1;
        }

        private static bool LicenseSatisfies(string? driverClass, string? minRequiredClass)
        {
            if (string.IsNullOrWhiteSpace(minRequiredClass)) return true;
            var d = LicenseRank(driverClass);
            var r = LicenseRank(minRequiredClass);
            return d >= 0 && r >= 0 && d >= r;
        }

        public TransportResponseDto Create(TransportCreateRequestDto dto)
        {
            var partner = _partnerRepo.GetById(dto.ProviderPartnerId);
            if (partner == null) throw new InvalidOperationException("ProviderPartner not found");

            var t = new Transport
            {
                TransportCode = $"T-{DateTime.UtcNow:yyyyMMddHHmmss}",
                DepotId = dto.DepotId,
                ProviderPartnerId = dto.ProviderPartnerId,
                Status = TransportStatus.Planned,
                StartTimePlanned = dto.StartTimePlanned,
                Notes = dto.Notes,
                Stops = new List<TransportStop>
                {
                    new TransportStop
                    {
                        Seq = 0,
                        StopType = TransportStopType.Depot,
                        AddressId = dto.DepotId,
                        Status = TransportStopStatus.Planned,
                        ServiceTimeMin = 0
                    }
                }
            };

            _transportRepo.Add(t);
            _logRepo.Add(new ShippingLog { TransportId = t.TransportId, Status = "Transport.Created", CreatedAt = DateTime.UtcNow });
            var id = t.TransportId;

            _event.Trigger(new EventNotifyTriggerDto
            {
                PartnerId = dto.ProviderPartnerId,
                EventType = EventTypes.TransportCreated,
                Title = $"Có chuyến đi mới Id:{id}",
                Content = $"Transport #{id} vừa được tạo.",
                OverrideRequireAcknowledge = true
            });

            var loaded = _transportRepo.GetDetail(id);
            return _mapper.Map<TransportResponseDto>(loaded!);
        }

        public TransportResponseDto? Get(int transportId)
        {
            var t = _transportRepo.GetDetail(transportId);
            return t == null ? null : _mapper.Map<TransportResponseDto>(t);
        }

        public List<TransportResponseDto> Query(DateOnly? date, string? status, int? vehicleId)
            => Query(date, status, vehicleId, null);

        public List<TransportResponseDto> Query(DateOnly? date, string? status, int? vehicleId, int? providerPartnerId)
        {
            var list = _transportRepo.Query(date, status, vehicleId);
            if (providerPartnerId is not null)
                list = list.Where(t => t.ProviderPartnerId == providerPartnerId.Value).ToList();

            return list.Select(_mapper.Map<TransportResponseDto>).ToList();
        }

        public void Assign(int transportId, TransportAssignRequestDto dto)
        {
            var t = _transportRepo.GetById(transportId) ?? throw new KeyNotFoundException("Transport not found");
            var v = _vehicleRepo.GetById(dto.VehicleId) ?? throw new InvalidOperationException("Vehicle not found");
            var dr = _driverRepo.GetById(dto.DriverId) ?? throw new InvalidOperationException("Driver not found");

            if (v.PartnerId != t.ProviderPartnerId) throw new InvalidOperationException("Vehicle partner mismatch");
            if (dr.PartnerId != t.ProviderPartnerId) throw new InvalidOperationException("Driver partner mismatch");

            var s = t.StartTimePlanned ?? DateTimeOffset.UtcNow;
            var e = t.EndTimePlanned;

            if (_transportRepo.VehicleBusy(dto.VehicleId, transportId, s, e)) throw new InvalidOperationException("Vehicle busy");
            if (_transportRepo.DriverBusy(dto.DriverId, transportId, s, e)) throw new InvalidOperationException("Driver busy");

            if (dto.PorterIds?.Any() == true)
            {
                var busy = _transportRepo.BusyPorters(dto.PorterIds, transportId, s, e);
                if (busy.Any()) throw new InvalidOperationException($"Porters busy: {string.Join(",", busy)}");
            }

            t.VehicleId = dto.VehicleId;
            t.DriverId = dto.DriverId;
            _transportRepo.Update(t);

            _transportRepo.ReplacePorters(transportId, dto.PorterIds ?? new());

            _transportRepo.UpdateStatus(transportId, TransportStatus.Assigned, null, null);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = "Transport.Assigned", CreatedAt = DateTime.UtcNow });
        }

        public void AddStops(int transportId, TransportAddStopsRequestDto dto)
        {
            var stops = dto.Stops.Select(s => new TransportStop
            {
                TransportId = transportId,
                Seq = s.Seq,
                StopType = Enum.Parse<TransportStopType>(s.StopType, true),
                AddressId = s.AddressId,
                ServiceTimeMin = s.ServiceTimeMin,
                Status = TransportStopStatus.Planned
            }).ToList();
            _transportRepo.AddStops(transportId, stops);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = "Transport.StopsUpdated", CreatedAt = DateTime.UtcNow });
        }

        public void AddInvoices(int transportId, TransportAddInvoicesRequestDto dto)
        {
            foreach (var id in dto.InvoiceIds)
                if (_invoiceRepo.GetById(id) == null) throw new InvalidOperationException($"Invoice {id} not found");

            if (_transportRepo.InvoiceAssignedElsewhere(dto.InvoiceIds, transportId))
                throw new InvalidOperationException("Có invoice đã gán transport khác");

            _transportRepo.AddInvoices(transportId, dto.InvoiceIds);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = "Transport.InvoicesUpdated", CreatedAt = DateTime.UtcNow });
        }

        public void ReplaceInvoices(int transportId, List<int> invoiceIds)
        {
            var list = invoiceIds ?? new List<int>();
            foreach (var id in list)
                if (_invoiceRepo.GetById(id) == null) throw new InvalidOperationException($"Invoice {id} not found");

            if (_transportRepo.InvoiceAssignedElsewhere(list, transportId))
                throw new InvalidOperationException("Có invoice đã gán transport khác");

            _transportRepo.ReplaceInvoices(transportId, list);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = "Transport.InvoicesUpdated", CreatedAt = DateTime.UtcNow });
        }

        public void Start(int transportId, DateTimeOffset at)
        {
            var t = _transportRepo.GetDetail(transportId) ?? throw new KeyNotFoundException();

            if (t.VehicleId == null || t.DriverId == null)
                throw new InvalidOperationException("Transport chưa được gán xe hoặc tài xế.");

            var vehicleId = t.VehicleId.Value;
            var driverId = t.DriverId.Value;

            var s = at;
            var e = t.EndTimePlanned;

            var v = _vehicleRepo.GetById(vehicleId) ?? throw new InvalidOperationException("Vehicle not found");
            var d = _driverRepo.GetById(driverId) ?? throw new InvalidOperationException("Driver not found");

            if (!LicenseSatisfies(d.LicenseClass, v.MinLicenseClass))
                throw new InvalidOperationException($"Driver {d.FullName} (license {d.LicenseClass}) không đạt yêu cầu {v.MinLicenseClass} của xe {v.Code}.");

            if (_transportRepo.VehicleBusy(vehicleId, transportId, s, e))
                throw new InvalidOperationException("Vehicle busy");

            if (_transportRepo.DriverBusy(driverId, transportId, s, e))
                throw new InvalidOperationException("Driver busy");

            var porterIds = _transportRepo.GetPorterIds(transportId);
            if (porterIds.Any() && _transportRepo.BusyPorters(porterIds, transportId, s, e).Any())
                throw new InvalidOperationException("Porter(s) busy");

            _transportRepo.UpdateStatus(transportId, TransportStatus.EnRoute, at, null);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = "Transport.Started", CreatedAt = DateTime.UtcNow });
        }

        public void ArriveStop(int transportId, TransportStopArriveRequestDto dto)
        {
            _transportRepo.UpdateStopArrival(transportId, dto.TransportStopId, dto.At);
            _logRepo.Add(new ShippingLog { TransportId = transportId, TransportStopId = dto.TransportStopId, Status = "Stop.Arrived", CreatedAt = DateTime.UtcNow });
        }

        public void CompleteStop(int transportId, TransportStopDoneRequestDto dto)
        {
            _transportRepo.UpdateStopDeparture(transportId, dto.TransportStopId, dto.At);
            _logRepo.Add(new ShippingLog { TransportId = transportId, TransportStopId = dto.TransportStopId, Status = "Stop.Done", CreatedAt = DateTime.UtcNow });
        }

        public void Complete(int transportId, DateTimeOffset at)
        {
            if (!_transportRepo.AllNonDepotStopsDone(transportId)) throw new InvalidOperationException("Stops not completed");
            _transportRepo.UpdateStatus(transportId, TransportStatus.Completed, null, at);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = "Transport.Completed", CreatedAt = DateTime.UtcNow });
        }

        public void Cancel(int transportId, string reason)
        {
            _transportRepo.Cancel(transportId, reason);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = $"Transport.Cancelled:{reason}", CreatedAt = DateTime.UtcNow });
        }

        public void DeleteStop(int transportId, int transportStopId)
        {
            _transportRepo.RemoveStop(transportId, transportStopId);
            _logRepo.Add(new ShippingLog { TransportId = transportId, TransportStopId = transportStopId, Status = "Stop.Deleted", CreatedAt = DateTime.UtcNow });
        }

        public void ClearStops(int transportId, bool keepDepot)
        {
            _transportRepo.ClearStops(transportId, keepDepot);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = keepDepot ? "Stops.ClearedKeepDepot" : "Stops.ClearedAll", CreatedAt = DateTime.UtcNow });
        }
    }
}
