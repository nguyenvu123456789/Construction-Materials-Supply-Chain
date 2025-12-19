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
        private readonly INotificationService _notificationService;

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
            INotificationService notificationService)
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
            _notificationService = notificationService;
        }

        private static readonly string[] LicenseOrder =
        {
            "A1","A","B1","B","C1","C","D1","D2","D","BE","C1E","CE","D1E","D2E","DE"
        };

        private static int GetLicenseRank(string? className)
        {
            if (string.IsNullOrWhiteSpace(className)) return -1;
            var normalized = className.Trim().ToUpperInvariant();
            for (int i = 0; i < LicenseOrder.Length; i++)
                if (LicenseOrder[i].Equals(normalized, StringComparison.OrdinalIgnoreCase))
                    return i;
            return -1;
        }

        private static bool IsLicenseSatisfied(string? driverClass, string? minRequiredClass)
        {
            if (string.IsNullOrWhiteSpace(minRequiredClass)) return true;
            var driverRank = GetLicenseRank(driverClass);
            var requiredRank = GetLicenseRank(minRequiredClass);
            return driverRank >= 0 && requiredRank >= 0 && driverRank >= requiredRank;
        }

        public TransportResponseDto Create(TransportCreateRequestDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var partner = _partnerRepo.GetById(dto.ProviderPartnerId);
            if (partner == null)
                throw new InvalidOperationException(TransportMessages.PROVIDER_PARTNER_NOT_FOUND);

            var warehouse = _warehouseRepo.GetById(dto.WarehouseId);
            if (warehouse == null)
                throw new InvalidOperationException(TransportMessages.WAREHOUSE_NOT_FOUND);

            var transport = new Transport
            {
                TransportCode = $"T-{DateTime.Now:yyyyMMddHHmmss}",
                WarehouseId = dto.WarehouseId,
                ProviderPartnerId = dto.ProviderPartnerId,
                Status = TransportStatus.Planned,
                StartTimePlanned = dto.StartTimePlanned,
                EndTimePlanned = dto.EndTimePlanned,
                Notes = dto.Notes,
            };

            _transportRepo.Add(transport);
            _logRepo.Add(new ShippingLog { TransportId = transport.TransportId, Status = "Transport.Created", CreatedAt = DateTime.Now });

            var transportId = transport.TransportId;
            _notificationService.Trigger(new EventNotifyTriggerDto
            {
                PartnerId = dto.ProviderPartnerId,
                EventType = EventTypes.TransportCreated,
                Title = $"Có chuyến đi mới Id:{transportId}",
                Content = $"Transport #{transportId} vừa được tạo.",
                OverrideRequireAcknowledge = true
            });

            var loadedTransport = _transportRepo.GetDetail(transportId);
            return _mapper.Map<TransportResponseDto>(loadedTransport!);
        }

        public TransportResponseDto? Get(int transportId)
        {
            var transport = _transportRepo.GetDetail(transportId);
            return transport == null ? null : _mapper.Map<TransportResponseDto>(transport);
        }

        public List<TransportResponseDto> Query(DateOnly? date, string? status, int? vehicleId)
            => Query(date, status, vehicleId, null);

        public List<TransportResponseDto> Query(DateOnly? date, string? status, int? vehicleId, int? providerPartnerId)
        {
            var transports = _transportRepo.Query(date, status, vehicleId);
            if (providerPartnerId.HasValue)
            {
                transports = transports.Where(t => t.ProviderPartnerId == providerPartnerId.Value).ToList();
            }

            return transports.Select(_mapper.Map<TransportResponseDto>).ToList();
        }

        public void Assign(int transportId, TransportAssignRequestDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var transport = _transportRepo.GetById(transportId)
                ?? throw new KeyNotFoundException(string.Format(TransportMessages.TRANSPORT_NOT_FOUND, transportId));

            var vehicle = _vehicleRepo.GetById(dto.VehicleId)
                ?? throw new InvalidOperationException(TransportMessages.VEHICLE_NOT_FOUND);

            var driver = _driverRepo.GetById(dto.DriverId)
                ?? throw new InvalidOperationException(TransportMessages.DRIVER_NOT_FOUND);

            if (vehicle.PartnerId != transport.ProviderPartnerId || driver.PartnerId != transport.ProviderPartnerId)
                throw new InvalidOperationException(TransportMessages.PARTNER_MISMATCH);

            var startTime = transport.StartTimePlanned ?? DateTimeOffset.Now;
            var endTime = transport.EndTimePlanned;

            if (_transportRepo.VehicleBusy(dto.VehicleId, transportId, startTime, endTime))
                throw new InvalidOperationException(TransportMessages.VEHICLE_BUSY);

            if (_transportRepo.DriverBusy(dto.DriverId, transportId, startTime, endTime))
                throw new InvalidOperationException(TransportMessages.DRIVER_BUSY);

            if (dto.PorterIds != null && dto.PorterIds.Any())
            {
                var busyPorters = _transportRepo.BusyPorters(dto.PorterIds, transportId, startTime, endTime);
                if (busyPorters.Any())
                {
                    var busyList = string.Join(", ", busyPorters);
                    throw new InvalidOperationException(string.Format(TransportMessages.PORTER_BUSY, busyList));
                }
            }

            transport.VehicleId = dto.VehicleId;
            transport.DriverId = dto.DriverId;
            _transportRepo.Update(transport);

            _transportRepo.ReplacePorters(transportId, dto.PorterIds ?? new List<int>());
            _transportRepo.UpdateStatus(transportId, TransportStatus.Assigned, null, null);

            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = "Transport.Assigned", CreatedAt = DateTime.Now });
        }

        public void AddStops(int transportId, TransportAddStopsRequestDto request)
        {
            if (request?.Stops == null) throw new ArgumentNullException(nameof(request));

            var transport = _transportRepo.GetDetail(transportId);
            if (transport == null)
                throw new Exception(string.Format(TransportMessages.TRANSPORT_NOT_FOUND, transportId));

            if (transport.Status != TransportStatus.Planned && transport.Status != TransportStatus.Assigned)
                throw new Exception(TransportMessages.TRANSPORT_ALREADY_STARTED_OR_COMPLETED);

            foreach (var item in request.Stops)
            {
                if (item.InvoiceIds == null || !item.InvoiceIds.Any())
                    throw new Exception(TransportMessages.STOP_MISSING_INVOICE);

                var invoices = _invoiceRepo.GetAll()
                                           .Where(i => item.InvoiceIds.Contains(i.InvoiceId))
                                           .ToList();

                if (invoices.Count != item.InvoiceIds.Count)
                    throw new Exception(string.Format(TransportMessages.INVOICE_NOT_FOUND, string.Join(",", item.InvoiceIds.Except(invoices.Select(x => x.InvoiceId)))));

                var addresses = invoices.Select(i => i.Address?.Trim().ToLower()).Distinct().ToList();
                if (addresses.Count > 1)
                {
                    throw new Exception(TransportMessages.INVOICE_ADDRESS_MISMATCH);
                }

                string targetAddress = invoices.First().Address;
                if (string.IsNullOrEmpty(targetAddress))
                    throw new Exception(string.Format(TransportMessages.INVOICE_NO_ADDRESS, invoices.First().InvoiceCode));

                var stop = new TransportStop
                {
                    TransportId = transportId,
                    Seq = item.Seq,
                    StopType = Enum.Parse<TransportStopType>(item.StopType),
                    Address = targetAddress,
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
            var cleanInvoiceIds = invoiceIds?.Distinct().ToList() ?? new List<int>();

            foreach (var id in cleanInvoiceIds)
            {
                var invoice = _invoiceRepo.GetById(id)
                    ?? throw new InvalidOperationException(string.Format(TransportMessages.INVOICE_NOT_FOUND, id));

                if (!string.Equals(invoice.InvoiceType, "Export", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException(string.Format(TransportMessages.INVOICE_NOT_EXPORT, id));
            }

            if (_transportRepo.InvoiceAssignedElsewhere(cleanInvoiceIds, transportId, transportStopId))
                throw new InvalidOperationException(string.Format(TransportMessages.INVOICE_ASSIGNED_ELSEWHERE, "Unknown"));

            _transportRepo.SetStopInvoices(transportId, transportStopId, cleanInvoiceIds);

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
            var transport = _transportRepo.GetDetail(transportId)
                ?? throw new KeyNotFoundException(string.Format(TransportMessages.TRANSPORT_NOT_FOUND, transportId));

            if (transport.VehicleId == null || transport.DriverId == null)
                throw new InvalidOperationException(TransportMessages.TRANSPORT_NOT_ASSIGNED);

            var vehicleId = transport.VehicleId.Value;
            var driverId = transport.DriverId.Value;
            var startTime = at;
            var endTime = transport.EndTimePlanned;

            var vehicle = _vehicleRepo.GetById(vehicleId)
                ?? throw new InvalidOperationException(TransportMessages.VEHICLE_NOT_FOUND);
            var driver = _driverRepo.GetById(driverId)
                ?? throw new InvalidOperationException(TransportMessages.DRIVER_NOT_FOUND);

            if (!IsLicenseSatisfied(driver.LicenseClass, vehicle.MinLicenseClass))
            {
                throw new InvalidOperationException(string.Format(TransportMessages.DRIVER_LICENSE_INVALID,
                    driver.FullName, driver.LicenseClass, vehicle.Code, vehicle.MinLicenseClass));
            }

            if (_transportRepo.VehicleBusy(vehicleId, transportId, startTime, endTime))
                throw new InvalidOperationException(TransportMessages.VEHICLE_BUSY);

            if (_transportRepo.DriverBusy(driverId, transportId, startTime, endTime))
                throw new InvalidOperationException(TransportMessages.DRIVER_BUSY);

            var porterIds = _transportRepo.GetPorterIds(transportId);
            if (porterIds.Any() && _transportRepo.BusyPorters(porterIds, transportId, startTime, endTime).Any())
                throw new InvalidOperationException(string.Format(TransportMessages.PORTER_BUSY, "Multiple"));

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
            if (!_transportRepo.AllNonDepotStopsDone(transportId))
                throw new InvalidOperationException(TransportMessages.TRANSPORT_STOPS_NOT_COMPLETED);

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
                throw new InvalidOperationException(TransportMessages.STOP_PROOF_BASE64_REQUIRED);

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

            var transport = stop.Transport;

            return new TransportInvoiceTrackingDto
            {
                TransportId = transport.TransportId,
                TransportCode = transport.TransportCode,
                Status = transport.Status.ToString(),
                DriverName = transport.Driver?.FullName,
                VehicleCode = transport.Vehicle?.Code,
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
        }

        public List<CustomerOrderStatusDto> GetHistory(int customerPartnerId)
        {
            var importInvoices = _invoiceRepo.GetCustomerExportInvoices(customerPartnerId);

            var orderIds = importInvoices.Select(i => i.OrderId).Distinct().ToList();
            var exportInvoices = _invoiceRepo.GetExportInvoicesByOrderIds(orderIds);

            var exportByOrder = exportInvoices
                .GroupBy(e => e.OrderId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<CustomerOrderStatusDto>();

            foreach (var importInv in importInvoices)
            {
                var relatedExports = exportByOrder.TryGetValue(importInv.OrderId, out var exps) ? exps : new List<Invoice>();

                var transports = new List<Transport>();
                foreach (var exp in relatedExports)
                {
                    var ts = _transportRepo.GetByInvoiceId(exp.InvoiceId);
                    if (ts.Count > 0) transports.AddRange(ts);
                }

                string deliveryStatus;
                if (!relatedExports.Any())
                {
                    deliveryStatus = "Chưa xuất kho";
                }
                else if (!transports.Any())
                {
                    deliveryStatus = "Chưa xếp chuyến";
                }
                else
                {
                    var exportIds = relatedExports.Select(e => e.InvoiceId).ToHashSet();
                    var relevantStops = transports
                        .SelectMany(t => t.Stops)
                        .Where(s => s.TransportInvoices.Any(ti => exportIds.Contains(ti.InvoiceId)))
                        .ToList();

                    var allDone = relevantStops.Any() && relevantStops.All(s => s.Status == TransportStopStatus.Done);
                    var anyEnRoute = transports.Any(t => t.Status == TransportStatus.EnRoute || t.Status == TransportStatus.Assigned);

                    if (allDone) deliveryStatus = "Đã giao xong";
                    else if (anyEnRoute) deliveryStatus = "Đang giao";
                    else deliveryStatus = "Đang xử lý";
                }

                result.Add(new CustomerOrderStatusDto
                {
                    InvoiceId = importInv.InvoiceId,
                    InvoiceCode = importInv.InvoiceCode,
                    IssueDate = importInv.IssueDate,
                    TotalAmount = importInv.TotalAmount,
                    InvoiceType = importInv.InvoiceType,
                    ExportStatus = relatedExports.FirstOrDefault()?.ExportStatus ?? "N/A",
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

            foreach (var driver in drivers)
            {
                var busyUntil = _transportRepo.DriverBusyUntil(driver.DriverId, start, endTime);
                result.Add(new ResourceStatusDto
                {
                    ResourceType = "Driver",
                    Id = driver.DriverId,
                    Name = driver.FullName,
                    Status = busyUntil == null ? "Free" : "Busy",
                    BusyUntil = busyUntil
                });
            }

            foreach (var vehicle in vehicles)
            {
                var busyUntil = _transportRepo.VehicleBusyUntil(vehicle.VehicleId, start, endTime);
                result.Add(new ResourceStatusDto
                {
                    ResourceType = "Vehicle",
                    Id = vehicle.VehicleId,
                    Name = vehicle.Code,
                    Status = busyUntil == null ? "Free" : "Busy",
                    BusyUntil = busyUntil
                });
            }

            foreach (var porter in porters)
            {
                var busyUntil = _transportRepo.PorterBusyUntil(porter.PorterId, start, endTime);
                result.Add(new ResourceStatusDto
                {
                    ResourceType = "Porter",
                    Id = porter.PorterId,
                    Name = porter.FullName,
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