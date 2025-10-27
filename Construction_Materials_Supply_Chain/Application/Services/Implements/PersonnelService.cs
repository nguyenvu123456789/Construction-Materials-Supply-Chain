using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class PersonnelService : IPersonnelService
    {
        private readonly IDriverRepository _drivers;
        private readonly IPorterRepository _porters;
        private readonly IVehicleRepository _vehicles;
        private readonly ITransportRepository _transport;

        public PersonnelService(IDriverRepository drivers, IPorterRepository porters, IVehicleRepository vehicles, ITransportRepository transport)
        {
            _drivers = drivers;
            _porters = porters;
            _vehicles = vehicles;
            _transport = transport;
        }

        public PersonResponseDto Create(PersonCreateDto dto)
        {
            var type = dto.Type?.Trim().ToLowerInvariant();
            if (type == "driver")
            {
                var d = new Driver
                {
                    FullName = dto.FullName,
                    Phone = dto.Phone,
                    Active = dto.Active,
                    PartnerId = dto.PartnerId
                };
                _drivers.Add(d);
                return new PersonResponseDto
                {
                    Type = "driver",
                    Id = d.DriverId,
                    FullName = d.FullName,
                    Phone = d.Phone,
                    Active = d.Active,
                    PartnerId = d.PartnerId
                };
            }
            if (type == "porter")
            {
                var p = new Porter
                {
                    FullName = dto.FullName,
                    Phone = dto.Phone,
                    Active = dto.Active,
                    PartnerId = dto.PartnerId
                };
                _porters.Add(p);
                return new PersonResponseDto
                {
                    Type = "porter",
                    Id = p.PorterId,
                    FullName = p.FullName,
                    Phone = p.Phone,
                    Active = p.Active,
                    PartnerId = p.PartnerId
                };
            }
            if (type == "vehicle")
            {
                var v = new Vehicle
                {
                    Code = dto.Code!,
                    PlateNumber = dto.PlateNumber!,
                    VehicleClass = dto.VehicleClass,
                    Active = dto.Active,
                    PartnerId = dto.PartnerId
                };
                _vehicles.Add(v);
                return new PersonResponseDto
                {
                    Type = "vehicle",
                    Id = v.VehicleId,
                    FullName = v.Code,
                    Active = v.Active,
                    Code = v.Code,
                    PlateNumber = v.PlateNumber,
                    VehicleClass = v.VehicleClass,
                    PartnerId = v.PartnerId
                };
            }
            throw new InvalidOperationException("type");
        }

        public PersonResponseDto? Get(string type, int id)
        {
            type = type.Trim().ToLowerInvariant();

            if (type == "driver")
            {
                var d = _drivers.GetById(id);
                return d == null ? null : new PersonResponseDto
                {
                    Type = "driver",
                    Id = d.DriverId,
                    PartnerId = d.PartnerId,
                    FullName = d.FullName,
                    Phone = d.Phone,
                    Active = d.Active,
                    Hometown = d.Hometown,
                    LicenseClass = d.LicenseClass,
                    BirthYear = d.BirthDate.HasValue ? d.BirthDate.Value.Year : (int?)null
                };
            }

            if (type == "porter")
            {
                var p = _porters.GetById(id);
                return p == null ? null : new PersonResponseDto
                {
                    Type = "porter",
                    Id = p.PorterId,
                    PartnerId = p.PartnerId,
                    FullName = p.FullName,
                    Phone = p.Phone,
                    Active = p.Active,
                    BirthYear = p.BirthYear,
                    Hometown = p.Hometown
                };
            }

            if (type == "vehicle")
            {
                var v = _vehicles.GetById(id);
                return v == null ? null : new PersonResponseDto
                {
                    Type = "vehicle",
                    Id = v.VehicleId,
                    PartnerId = v.PartnerId,
                    FullName = v.Code,
                    Active = v.Active,
                    Code = v.Code,
                    PlateNumber = v.PlateNumber,
                    VehicleClass = v.VehicleClass,
                    MinLicenseClass = v.MinLicenseClass,
                    CapacityTon = v.PayloadTons
                };
            }

            return null;
        }

        public List<PersonResponseDto> GetAll(string type, int? partnerId)
        {
            type = type.Trim().ToLowerInvariant();

            if (type == "driver")
            {
                var q = _drivers.GetAll().AsQueryable();
                if (partnerId.HasValue) q = q.Where(x => x.PartnerId == partnerId.Value);
                return q
                    .Select(d => new PersonResponseDto
                    {
                        Type = "driver",
                        Id = d.DriverId,
                        PartnerId = d.PartnerId,
                        FullName = d.FullName,
                        Phone = d.Phone,
                        Active = d.Active,
                        Hometown = d.Hometown,
                        LicenseClass = d.LicenseClass,
                        BirthYear = d.BirthDate.HasValue ? d.BirthDate.Value.Year : (int?)null
                    })
                    .ToList();
            }

            if (type == "porter")
            {
                var q = _porters.GetAll().AsQueryable();
                if (partnerId.HasValue) q = q.Where(x => x.PartnerId == partnerId.Value);
                return q
                    .Select(p => new PersonResponseDto
                    {
                        Type = "porter",
                        Id = p.PorterId,
                        PartnerId = p.PartnerId,
                        FullName = p.FullName,
                        Phone = p.Phone,
                        Active = p.Active,
                        BirthYear = p.BirthYear,
                        Hometown = p.Hometown
                    })
                    .ToList();
            }

            if (type == "vehicle")
            {
                var q = _vehicles.GetAll().AsQueryable();
                if (partnerId.HasValue) q = q.Where(x => x.PartnerId == partnerId.Value);
                return q
                    .Select(v => new PersonResponseDto
                    {
                        Type = "vehicle",
                        Id = v.VehicleId,
                        PartnerId = v.PartnerId,
                        FullName = v.Code,
                        Active = v.Active,
                        Code = v.Code,
                        PlateNumber = v.PlateNumber,
                        VehicleClass = v.VehicleClass,
                        MinLicenseClass = v.MinLicenseClass,
                        CapacityTon = v.PayloadTons
                    })
                    .ToList();
            }

            return new List<PersonResponseDto>();
        }

        public List<PersonResponseDto> Search(string type, string? q, bool? active, int? top, int? partnerId)
        {
            type = type.Trim().ToLowerInvariant();

            if (type == "driver")
            {
                var list = _drivers.Search(q, active, top, partnerId);
                return list
                    .Select(d => new PersonResponseDto
                    {
                        Type = "driver",
                        Id = d.DriverId,
                        PartnerId = d.PartnerId,
                        FullName = d.FullName,
                        Phone = d.Phone,
                        Active = d.Active,
                        Hometown = d.Hometown,
                        LicenseClass = d.LicenseClass,
                        BirthYear = d.BirthDate.HasValue ? d.BirthDate.Value.Year : (int?)null
                    })
                    .ToList();
            }

            if (type == "porter")
            {
                var list = _porters.Search(q, active, top, partnerId);
                return list
                    .Select(p => new PersonResponseDto
                    {
                        Type = "porter",
                        Id = p.PorterId,
                        PartnerId = p.PartnerId,
                        FullName = p.FullName,
                        Phone = p.Phone,
                        Active = p.Active,
                        BirthYear = p.BirthYear,
                        Hometown = p.Hometown
                    })
                    .ToList();
            }

            if (type == "vehicle")
            {
                var list = _vehicles.Search(q, active, top, partnerId);
                return list
                    .OrderBy(x => x.Code)
                    .Select(v => new PersonResponseDto
                    {
                        Type = "vehicle",
                        Id = v.VehicleId,
                        PartnerId = v.PartnerId,
                        FullName = v.Code,
                        Active = v.Active,
                        Code = v.Code,
                        PlateNumber = v.PlateNumber,
                        VehicleClass = v.VehicleClass,
                        MinLicenseClass = v.MinLicenseClass,
                        CapacityTon = v.PayloadTons
                    })
                    .ToList();
            }

            return new List<PersonResponseDto>();
        }

        public void Update(string type, int id, PersonUpdateDto dto)
        {
            type = type.Trim().ToLowerInvariant();

            if (type == "driver")
            {
                var d = _drivers.GetById(id) ?? throw new KeyNotFoundException();
                d.FullName = dto.FullName;
                d.Phone = dto.Phone;
                d.Active = dto.Active;
                d.Hometown = dto.Hometown ?? d.Hometown;
                d.LicenseClass = dto.LicenseClass ?? d.LicenseClass;
                if (dto.BirthYear.HasValue)
                {
                    var month = d.BirthDate.HasValue ? d.BirthDate.Value.Month : 1;
                    var day = d.BirthDate.HasValue ? d.BirthDate.Value.Day : 1;
                    d.BirthDate = new DateOnly(dto.BirthYear.Value, month, day);
                }
                _drivers.Update(d);
                return;
            }

            if (type == "porter")
            {
                var p = _porters.GetById(id) ?? throw new KeyNotFoundException();
                p.FullName = dto.FullName;
                p.Phone = dto.Phone;
                p.Active = dto.Active;
                p.BirthYear = dto.BirthYear;
                p.Hometown = dto.Hometown ?? p.Hometown;
                _porters.Update(p);
                return;
            }

            if (type == "vehicle")
            {
                var v = _vehicles.GetById(id) ?? throw new KeyNotFoundException();
                v.Code = dto.Code ?? v.Code;
                v.PlateNumber = dto.PlateNumber ?? v.PlateNumber;
                v.VehicleClass = dto.VehicleClass ?? v.VehicleClass;
                v.Active = dto.Active;
                v.MinLicenseClass = dto.MinLicenseClass ?? v.MinLicenseClass;
                v.PayloadTons = dto.CapacityTon.Value;
                _vehicles.Update(v);
                return;
            }

            throw new InvalidOperationException("type");
        }

        public void Delete(string type, int id)
        {
            type = type.Trim().ToLowerInvariant();

            if (type == "driver")
            {
                var d = _drivers.GetById(id) ?? throw new KeyNotFoundException();
                _drivers.Delete(d);
                return;
            }

            if (type == "porter")
            {
                var p = _porters.GetById(id) ?? throw new KeyNotFoundException();
                _porters.Delete(p);
                return;
            }

            if (type == "vehicle")
            {
                var v = _vehicles.GetById(id) ?? throw new KeyNotFoundException();
                _vehicles.Delete(v);
                return;
            }

            throw new InvalidOperationException("type");
        }

        public AvailabilityResponseDto GetAvailability(string type, DateTimeOffset at, int durationMin)
        {
            var end = at.AddMinutes(durationMin);
            var res = new AvailabilityResponseDto();

            if (type.Equals("driver", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var d in _drivers.GetAll().Where(x => x.Active))
                {
                    var busyUntil = _transport.DriverBusyUntil(d.DriverId, at, end);
                    var item = new AvailabilityItemDto
                    {
                        Type = "driver",
                        Id = d.DriverId,
                        NameOrCode = d.FullName,
                        FreeNow = busyUntil == null,
                        AvailableAt = busyUntil
                    };
                    (busyUntil == null ? res.Free : res.Busy).Add(item);
                }
            }
            else if (type.Equals("porter", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var p in _porters.GetAll().Where(x => x.Active))
                {
                    var busyUntil = _transport.PorterBusyUntil(p.PorterId, at, end);
                    var item = new AvailabilityItemDto
                    {
                        Type = "porter",
                        Id = p.PorterId,
                        NameOrCode = p.FullName,
                        FreeNow = busyUntil == null,
                        AvailableAt = busyUntil
                    };
                    (busyUntil == null ? res.Free : res.Busy).Add(item);
                }
            }
            else if (type.Equals("vehicle", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var v in _vehicles.GetAll().Where(x => x.Active))
                {
                    var busyUntil = _transport.VehicleBusyUntil(v.VehicleId, at, end);
                    var item = new AvailabilityItemDto
                    {
                        Type = "vehicle",
                        Id = v.VehicleId,
                        NameOrCode = v.Code,
                        Plate = v.PlateNumber,
                        FreeNow = busyUntil == null,
                        AvailableAt = busyUntil
                    };
                    (busyUntil == null ? res.Free : res.Busy).Add(item);
                }
            }

            res.Free = res.Free.OrderBy(x => x.NameOrCode).ToList();
            res.Busy = res.Busy.OrderBy(x => x.AvailableAt).ThenBy(x => x.NameOrCode).ToList();
            return res;
        }
    }
}
