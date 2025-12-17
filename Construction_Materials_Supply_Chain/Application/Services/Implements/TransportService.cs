using Application.Constants.Messages;
using Application.DTOs;
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
        private readonly IWarehouseRepository _warehouseRepo;
        private readonly IMapper _mapper;
        private readonly INotificationService _event;

        public TransportService(
            ITransportRepository transportRepo,
            IInvoiceRepository invoiceRepo,
            IShippingLogRepository logRepo,
            IDriverRepository driverRepo,
            IPorterRepository porterRepo,
            IVehicleRepository vehicleRepo,
            IPartnerRepository partnerRepo,
            IWarehouseRepository warehouseRepo,
            IMapper mapper,
            INotificationService eventSvc)
        {
            _transportRepo = transportRepo;
            _invoiceRepo = invoiceRepo;
            _logRepo = logRepo;
            _driverRepo = driverRepo;
            _porterRepo = porterRepo;
            _vehicleRepo = vehicleRepo;
            _partnerRepo = partnerRepo;
            _warehouseRepo = warehouseRepo;
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

            var warehouse = _warehouseRepo.GetById(dto.WarehouseId);
            if (warehouse == null) throw new InvalidOperationException("Warehouse not found");

            var t = new Transport
            {
                TransportCode = $"T-{DateTime.Now:yyyyMMddHHmmss}",
                WarehouseId = dto.WarehouseId,
                ProviderPartnerId = dto.ProviderPartnerId,
                Status = TransportStatus.Planned,
                StartTimePlanned = dto.StartTimePlanned,
                Notes = dto.Notes,
            };

            _transportRepo.Add(t);
            _logRepo.Add(new ShippingLog { TransportId = t.TransportId, Status = "Transport.Created", CreatedAt = DateTime.Now });
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

            var s = t.StartTimePlanned ?? DateTimeOffset.Now;
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
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = "Transport.Assigned", CreatedAt = DateTime.Now });
        }

        public void AddStops(int transportId, TransportAddStopsRequestDto request)
        {
            var transport = _transportRepo.GetDetail(transportId);
            if (transport == null)
                throw new Exception("Transport not found");

            if (transport.Status != TransportStatus.Planned && transport.Status != TransportStatus.Assigned)
                throw new Exception("Cannot add stops to a transport that is already in progress or completed.");

            foreach (var item in request.Stops)
            {
                if (item.InvoiceIds == null || !item.InvoiceIds.Any())
                    throw new Exception("A stop must have at least one invoice assigned.");

                var invoices = _invoiceRepo.GetAll()
                                           .Where(i => item.InvoiceIds.Contains(i.InvoiceId))
                                           .ToList();

                if (invoices.Count != item.InvoiceIds.Count)
                    throw new Exception("One or more Invoice IDs provided do not exist.");

                var addresses = invoices.Select(i => i.Address?.Trim().ToLower()).Distinct().ToList();
                if (addresses.Count > 1)
                {
                    throw new Exception($"Cannot create a single stop for Invoices {string.Join(", ", item.InvoiceIds)} because they have different delivery addresses.");
                }

                string targetAddressStr = invoices.First().Address;
                if (string.IsNullOrEmpty(targetAddressStr))
                    throw new Exception($"Invoice {invoices.First().InvoiceCode} does not have a valid address.");

                var stop = new TransportStop
                {
                    TransportId = transportId,
                    Seq = item.Seq,
                    StopType = Enum.Parse<TransportStopType>(item.StopType),

                    Address = targetAddressStr,

                    ServiceTimeMin = item.ServiceTimeMin,
                    Status = TransportStopStatus.Planned,
                    TransportInvoices = invoices.Select(inv => new TransportInvoice
                    {
                        InvoiceId = inv.InvoiceId
                    }).ToList()
                };

                transport.Stops.Add(stop);
            }

            _transportRepo.Update(transport);
        }

        public void SetStopInvoices(int transportId, int transportStopId, List<int> invoiceIds)
        {
            var list = invoiceIds?.Distinct().ToList() ?? new List<int>();
            foreach (var id in list)
            {
                var inv = _invoiceRepo.GetById(id) ?? throw new InvalidOperationException($"Invoice {id} not found");
                if (!string.Equals(inv.InvoiceType, "Export", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Invoice {id} không phải Export");
            }

            if (_transportRepo.InvoiceAssignedElsewhere(list, transportId, transportStopId))
                throw new InvalidOperationException("Có invoice đã gán stop khác");

            _transportRepo.SetStopInvoices(transportId, transportStopId, list);
            _logRepo.Add(new ShippingLog
            {
                TransportId = transportId,
                TransportStopId = transportStopId,
                Status = "Stop.InvoicesUpdated",
                CreatedAt = DateTime.Now
            });
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
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = "Transport.Started", CreatedAt = DateTime.Now });
        }

        public void ArriveStop(int transportId, TransportStopArriveRequestDto dto)
        {
            _transportRepo.UpdateStopArrival(transportId, dto.TransportStopId, dto.At);
            _logRepo.Add(new ShippingLog { TransportId = transportId, TransportStopId = dto.TransportStopId, Status = "Stop.Arrived", CreatedAt = DateTime.Now });
        }

        public void CompleteStop(int transportId, TransportStopDoneRequestDto dto)
        {
            _transportRepo.UpdateStopDeparture(transportId, dto.TransportStopId, dto.At);
            _logRepo.Add(new ShippingLog { TransportId = transportId, TransportStopId = dto.TransportStopId, Status = "Stop.Done", CreatedAt = DateTime.Now });
        }

        public void Complete(int transportId, DateTimeOffset at)
        {
            if (!_transportRepo.AllNonDepotStopsDone(transportId)) throw new InvalidOperationException("Stops not completed");
            _transportRepo.UpdateStatus(transportId, TransportStatus.Completed, null, at);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = "Transport.Completed", CreatedAt = DateTime.Now });
        }

        public void Cancel(int transportId, string reason)
        {
            _transportRepo.Cancel(transportId, reason);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = $"Transport.Cancelled:{reason}", CreatedAt = DateTime.Now });
        }

        public void DeleteStop(int transportId, int transportStopId)
        {
            _transportRepo.RemoveStop(transportId, transportStopId);
            _logRepo.Add(new ShippingLog { TransportId = transportId, TransportStopId = transportStopId, Status = "Stop.Deleted", CreatedAt = DateTime.Now });
        }

        public void ClearStops(int transportId, bool keepDepot)
        {
            _transportRepo.ClearStops(transportId, keepDepot);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = keepDepot ? "Stops.ClearedKeepDepot" : "Stops.ClearedAll", CreatedAt = DateTime.Now });
        }

        public void UploadStopProofBase64(int transportId, TransportStopProofBase64Dto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Base64))
                throw new InvalidOperationException("Base64 is required");

            _transportRepo.UpdateStopProofImage(transportId, dto.TransportStopId, dto.Base64);
            _logRepo.Add(new ShippingLog
            {
                TransportId = transportId,
                TransportStopId = dto.TransportStopId,
                Status = "Stop.ProofUploaded",
                CreatedAt = DateTime.Now
            });
        }

        public TransportInvoiceTrackingDto? TrackInvoice(int invoiceId)
        {
            var stop = _transportRepo.GetStopByInvoice(invoiceId);
            if (stop == null) return null;

            var t = stop.Transport;

            var dto = new TransportInvoiceTrackingDto
            {
                TransportId = t.TransportId,
                TransportCode = t.TransportCode,
                Status = t.Status.ToString(),
                DriverName = t.Driver != null ? t.Driver.FullName : null,
                VehicleCode = t.Vehicle != null ? t.Vehicle.Code : null,
                Stop = new TrackingStopDto
                {
                    StopId = stop.TransportStopId,
                    StopType = stop.StopType.ToString(),
                    Address = stop.Address,

                    ETA = stop.ETA,
                    ATA = stop.ATA,
                    ATD = stop.ATD,
                    DeliveryPhotoBase64 = stop.ProofImageBase64
                }
            };

            return dto;
        }

        public List<CustomerOrderStatusDto> GetHistory(int customerPartnerId)
        {
            var imports = _invoiceRepo.GetCustomerExportInvoices(customerPartnerId);

            var orderIds = imports
                .Select(i => i.OrderId)
                .Distinct()
                .ToList();

            var exportInvoices = _invoiceRepo.GetExportInvoicesByOrderIds(orderIds);

            var exportByOrder = exportInvoices
                .GroupBy(e => e.OrderId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<CustomerOrderStatusDto>();

            foreach (var inv in imports)
            {
                var exportList = new List<Invoice>();
                if (exportByOrder.TryGetValue(inv.OrderId, out var exps))
                    exportList = exps;

                var transportList = new List<Transport>();
                foreach (var exp in exportList)
                {
                    var ts = _transportRepo.GetByInvoiceId(exp.InvoiceId);
                    if (ts.Count > 0)
                        transportList.AddRange(ts);
                }

                string deliveryStatus;

                if (!exportList.Any())
                {
                    deliveryStatus = "Chưa xuất kho";
                }
                else if (!transportList.Any())
                {
                    deliveryStatus = "Chưa xếp chuyến";
                }
                else
                {
                    var exportIds = exportList.Select(e => e.InvoiceId).ToHashSet();

                    var allStops = transportList
                        .SelectMany(t => t.Stops)
                        .Where(s => s.TransportInvoices.Any(ti => exportIds.Contains(ti.InvoiceId)))
                        .ToList();

                    var allDone = allStops.Any() && allStops.All(s => s.Status == TransportStopStatus.Done);
                    var anyEnRoute = transportList.Any(t =>
                        t.Status == TransportStatus.EnRoute ||
                        t.Status == TransportStatus.Assigned);

                    if (allDone)
                        deliveryStatus = "Đã giao xong";
                    else if (anyEnRoute)
                        deliveryStatus = "Đang giao";
                    else
                        deliveryStatus = "Đang xử lý";
                }

                result.Add(new CustomerOrderStatusDto
                {
                    InvoiceId = inv.InvoiceId,
                    InvoiceCode = inv.InvoiceCode,
                    IssueDate = inv.IssueDate,
                    TotalAmount = inv.TotalAmount,
                    InvoiceType = inv.InvoiceType,
                    ExportStatus = exportList.FirstOrDefault()?.ExportStatus ?? "N/A",
                    DeliveryStatus = deliveryStatus
                });
            }

            return result;
        }

        public List<ResourceStatusDto> GetResourcesStatus(DateTimeOffset start, DateTimeOffset? end, int providerPartnerId)
        {
            var result = new List<ResourceStatusDto>();
            var endTime = end ?? DateTimeOffset.MaxValue;

            var drivers = _driverRepo.Search(null, true, null, providerPartnerId);
            var vehicles = _vehicleRepo.Search(null, true, null, providerPartnerId);
            var porters = _porterRepo.Search(null, true, null, providerPartnerId);

            foreach (var dr in drivers)
            {
                var busyUntil = _transportRepo.DriverBusyUntil(dr.DriverId, start, endTime);
                result.Add(new ResourceStatusDto
                {
                    ResourceType = "Driver",
                    Id = dr.DriverId,
                    Name = dr.FullName,
                    Status = busyUntil == null ? "Free" : "Busy",
                    BusyUntil = busyUntil
                });
            }

            foreach (var v in vehicles)
            {
                var busyUntil = _transportRepo.VehicleBusyUntil(v.VehicleId, start, endTime);
                result.Add(new ResourceStatusDto
                {
                    ResourceType = "Vehicle",
                    Id = v.VehicleId,
                    Name = v.Code,
                    Status = busyUntil == null ? "Free" : "Busy",
                    BusyUntil = busyUntil
                });
            }

            foreach (var p in porters)
            {
                var busyUntil = _transportRepo.PorterBusyUntil(p.PorterId, start, endTime);
                result.Add(new ResourceStatusDto
                {
                    ResourceType = "Porter",
                    Id = p.PorterId,
                    Name = p.FullName,
                    Status = busyUntil == null ? "Free" : "Busy",
                    BusyUntil = busyUntil
                });
            }

            return result
                    .OrderByDescending(x => x.Status == "Free")
                    .ThenBy(x => x.ResourceType)
                    .ThenBy(x => x.Name)
                    .ToList();
        }
    }
}