using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class TransportService : ITransportService
    {
        private readonly ITransportRepository _transportRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IShippingLogRepository _logRepo;
        private readonly IDriverRepository _driverRepo;
        private readonly IPorterRepository _porterRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IMapper _mapper;

        public TransportService(
            ITransportRepository transportRepo,
            IOrderRepository orderRepo,
            IShippingLogRepository logRepo,
            IDriverRepository driverRepo,
            IPorterRepository porterRepo,
            IVehicleRepository vehicleRepo,
            IMapper mapper)
        {
            _transportRepo = transportRepo;
            _orderRepo = orderRepo;
            _logRepo = logRepo;
            _driverRepo = driverRepo;
            _porterRepo = porterRepo;
            _vehicleRepo = vehicleRepo;
            _mapper = mapper;
        }

        public TransportResponseDto Create(TransportCreateRequestDto dto)
        {
            var t = new Transport
            {
                TransportCode = $"T-{DateTime.UtcNow:yyyyMMddHHmmss}",
                DepotId = dto.DepotId,
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

            var loaded = _transportRepo.GetDetail(t.TransportId);
            return _mapper.Map<TransportResponseDto>(loaded!);
        }

        public TransportResponseDto? Get(int transportId)
        {
            var t = _transportRepo.GetDetail(transportId);
            return t == null ? null : _mapper.Map<TransportResponseDto>(t);
        }

        public List<TransportResponseDto> Query(DateOnly? date, string? status, int? vehicleId)
        {
            var list = _transportRepo.Query(date, status, vehicleId);
            return list.Select(_mapper.Map<TransportResponseDto>).ToList();
        }

        public void Assign(int transportId, TransportAssignRequestDto dto)
        {
            var t = _transportRepo.GetById(transportId) ?? throw new KeyNotFoundException();
            var s = t.StartTimePlanned ?? DateTimeOffset.UtcNow;
            var e = t.EndTimePlanned;

            var providerPartnerId = (t as dynamic).ProviderPartnerId;

            if (dto.VehicleId.HasValue)
            {
                var v = _vehicleRepo.GetById(dto.VehicleId.Value) ?? throw new InvalidOperationException("Vehicle not found");
                if (v.PartnerId != providerPartnerId)
                    throw new InvalidOperationException("Vehicle partner mismatch");
            }
            if (dto.DriverId.HasValue)
            {
                var d = _driverRepo.GetById(dto.DriverId.Value) ?? throw new InvalidOperationException("Driver not found");
                if (d.PartnerId != providerPartnerId)
                    throw new InvalidOperationException("Driver partner mismatch");
            }
            if (dto.PorterIds?.Any() == true)
            {
                var ps = _porterRepo.GetByIds(dto.PorterIds);
                var wrong = ps.Where(p => p.PartnerId != providerPartnerId).Select(p => p.PorterId).ToList();
                if (wrong.Any())
                    throw new InvalidOperationException($"Porter partner mismatch: {string.Join(",", wrong)}");
            }

            if (dto.VehicleId.HasValue && _transportRepo.VehicleBusy(dto.VehicleId.Value, transportId, s, e))
                throw new InvalidOperationException("Vehicle is busy in the selected time window");
            if (dto.DriverId.HasValue && _transportRepo.DriverBusy(dto.DriverId.Value, transportId, s, e))
                throw new InvalidOperationException("Driver is busy in the selected time window");
            var busy = (dto.PorterIds?.Count > 0)
                ? _transportRepo.BusyPorters(dto.PorterIds, transportId, s, e)
                : new List<int>();
            if (busy.Any())
                throw new InvalidOperationException($"Porters busy: {string.Join(",", busy)}");

            _transportRepo.Assign(transportId, dto.VehicleId ?? 0, dto.DriverId ?? 0, dto.PorterIds ?? new List<int>());
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

        public void AddOrders(int transportId, TransportAddOrdersRequestDto dto)
        {
            foreach (var id in dto.OrderIds)
                if (_orderRepo.GetById(id) == null) throw new InvalidOperationException($"Order {id} not found");
            var items = dto.OrderIds.Select(id => new TransportOrder { TransportId = transportId, OrderId = id }).ToList();
            _transportRepo.AddOrders(transportId, items);
            _logRepo.Add(new ShippingLog { TransportId = transportId, Status = "Transport.OrdersUpdated", CreatedAt = DateTime.UtcNow });
        }

        public void Start(int transportId, DateTimeOffset at)
        {
            var t = _transportRepo.GetById(transportId) ?? throw new KeyNotFoundException();
            var s = at;
            var e = t.EndTimePlanned;

            if (t.VehicleId.HasValue && _transportRepo.VehicleBusy(t.VehicleId.Value, transportId, s, e))
                throw new InvalidOperationException("Vehicle is busy");
            if (t.DriverId.HasValue && _transportRepo.DriverBusy(t.DriverId.Value, transportId, s, e))
                throw new InvalidOperationException("Driver is busy");

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
