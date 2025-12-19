using Application.Constants.Messages;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;
using FluentValidation;

namespace Application.Services.Implements
{
    public class PersonnelService : IPersonnelService
    {
        private readonly IDriverRepository _drivers;
        private readonly IPorterRepository _porters;
        private readonly IVehicleRepository _vehicles;
        private readonly ITransportRepository _transport;
        private readonly IValidator<PersonCreateDto> _createValidator;
        private readonly IValidator<PersonUpdateDto> _updateValidator;

        public PersonnelService(
            IDriverRepository drivers,
            IPorterRepository porters,
            IVehicleRepository vehicles,
            ITransportRepository transport,
            IValidator<PersonCreateDto> createValidator,
            IValidator<PersonUpdateDto> updateValidator)
        {
            _drivers = drivers;
            _porters = porters;
            _vehicles = vehicles;
            _transport = transport;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public PersonResponseDto Create(PersonCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto), PersonnelMessages.REQUEST_NULL);

            var validationResult = _createValidator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var type = dto.Type?.Trim().ToLowerInvariant();

            if (type == "driver")
            {
                if (_drivers.CheckExists(x => x.Phone == dto.Phone))
                    throw new InvalidOperationException(string.Format(PersonnelMessages.PHONE_EXISTED, dto.Phone));

                var d = new Driver
                {
                    FullName = dto.FullName,
                    Phone = dto.Phone,
                    Active = dto.Active,
                    PartnerId = dto.PartnerId,
                    Hometown = dto.Hometown,
                    BirthDate = dto.BirthYear.HasValue ? new DateOnly(dto.BirthYear.Value, 1, 1) : null,
                    LicenseClass = dto.LicenseClass
                };
                _drivers.Add(d);

                return new PersonResponseDto
                {
                    Type = "driver",
                    Id = d.DriverId,
                    FullName = d.FullName,
                    Phone = d.Phone,
                    Active = d.Active,
                    PartnerId = d.PartnerId,
                    Hometown = d.Hometown,
                    BirthYear = d.BirthDate.HasValue ? d.BirthDate.Value.Year : (int?)null,
                    LicenseClass = d.LicenseClass
                };
            }

            if (type == "porter")
            {
                if (_porters.CheckExists(x => x.Phone == dto.Phone))
                    throw new InvalidOperationException(string.Format(PersonnelMessages.PHONE_EXISTED, dto.Phone));

                var p = new Porter
                {
                    FullName = dto.FullName,
                    Phone = dto.Phone,
                    Active = dto.Active,
                    PartnerId = dto.PartnerId,
                    Hometown = dto.Hometown,
                    BirthYear = dto.BirthYear
                };
                _porters.Add(p);

                return new PersonResponseDto
                {
                    Type = "porter",
                    Id = p.PorterId,
                    FullName = p.FullName,
                    Phone = p.Phone,
                    Active = p.Active,
                    PartnerId = p.PartnerId,
                    Hometown = p.Hometown,
                    BirthYear = p.BirthYear
                };
            }

            if (type == "vehicle")
            {
                if (_vehicles.CheckExists(x => x.Code == dto.Code))
                    throw new InvalidOperationException(string.Format(PersonnelMessages.CODE_EXISTED, dto.Code));

                if (_vehicles.CheckExists(x => x.PlateNumber == dto.PlateNumber))
                    throw new InvalidOperationException(string.Format(PersonnelMessages.PLATE_EXISTED, dto.PlateNumber));

                var v = new Vehicle
                {
                    Code = dto.Code!,
                    PlateNumber = dto.PlateNumber!,
                    VehicleClass = dto.VehicleClass,
                    Active = dto.Active,
                    PartnerId = dto.PartnerId,
                    PayloadTons = dto.CapacityTon.HasValue ? dto.CapacityTon.Value : 0m,
                    MinLicenseClass = dto.MinLicenseClass
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
                    CapacityTon = v.PayloadTons,
                    PartnerId = v.PartnerId,
                    MinLicenseClass = v.MinLicenseClass
                };
            }

            throw new ArgumentException(PersonnelMessages.TYPE_INVALID);
        }

        public PersonResponseDto? Get(string type, int id)
        {
            var normalizedType = type?.Trim().ToLowerInvariant();

            if (normalizedType == "driver")
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

            if (normalizedType == "porter")
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

            if (normalizedType == "vehicle")
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
            var normalizedType = type?.Trim().ToLowerInvariant();

            if (normalizedType == "driver")
            {
                var q = _drivers.GetAll().AsQueryable();
                if (partnerId.HasValue) q = q.Where(x => x.PartnerId == partnerId.Value);
                return q.Select(d => new PersonResponseDto
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
                }).ToList();
            }

            if (normalizedType == "porter")
            {
                var q = _porters.GetAll().AsQueryable();
                if (partnerId.HasValue) q = q.Where(x => x.PartnerId == partnerId.Value);
                return q.Select(p => new PersonResponseDto
                {
                    Type = "porter",
                    Id = p.PorterId,
                    PartnerId = p.PartnerId,
                    FullName = p.FullName,
                    Phone = p.Phone,
                    Active = p.Active,
                    BirthYear = p.BirthYear,
                    Hometown = p.Hometown
                }).ToList();
            }

            if (normalizedType == "vehicle")
            {
                var q = _vehicles.GetAll().AsQueryable();
                if (partnerId.HasValue) q = q.Where(x => x.PartnerId == partnerId.Value);
                return q.Select(v => new PersonResponseDto
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
                }).ToList();
            }

            return new List<PersonResponseDto>();
        }

        public List<PersonResponseDto> Search(string type, string? q, bool? active, int? top, int? partnerId)
        {
            var normalizedType = type?.Trim().ToLowerInvariant();

            if (normalizedType == "driver")
            {
                var list = _drivers.Search(q, active, top, partnerId);
                return list.Select(d => new PersonResponseDto
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
                }).ToList();
            }

            if (normalizedType == "porter")
            {
                var list = _porters.Search(q, active, top, partnerId);
                return list.Select(p => new PersonResponseDto
                {
                    Type = "porter",
                    Id = p.PorterId,
                    PartnerId = p.PartnerId,
                    FullName = p.FullName,
                    Phone = p.Phone,
                    Active = p.Active,
                    BirthYear = p.BirthYear,
                    Hometown = p.Hometown
                }).ToList();
            }

            if (normalizedType == "vehicle")
            {
                var list = _vehicles.Search(q, active, top, partnerId);
                return list.OrderBy(x => x.Code).Select(v => new PersonResponseDto
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
                }).ToList();
            }

            return new List<PersonResponseDto>();
        }

        public void Update(string type, int id, PersonUpdateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto), PersonnelMessages.REQUEST_NULL);

            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var normalizedType = type?.Trim().ToLowerInvariant();

            if (normalizedType == "driver")
            {
                var d = _drivers.GetById(id)
                    ?? throw new KeyNotFoundException(string.Format(PersonnelMessages.DRIVER_NOT_FOUND, id));

                if (!string.IsNullOrEmpty(dto.Phone) && dto.Phone != d.Phone)
                {
                    if (_drivers.CheckExists(x => x.Phone == dto.Phone && x.DriverId != id))
                        throw new InvalidOperationException(string.Format(PersonnelMessages.PHONE_EXISTED, dto.Phone));
                }

                d.FullName = dto.FullName ?? d.FullName;
                d.Phone = dto.Phone ?? d.Phone;
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

            if (normalizedType == "porter")
            {
                var p = _porters.GetById(id)
                    ?? throw new KeyNotFoundException(string.Format(PersonnelMessages.PORTER_NOT_FOUND, id));

                if (!string.IsNullOrEmpty(dto.Phone) && dto.Phone != p.Phone)
                {
                    if (_porters.CheckExists(x => x.Phone == dto.Phone && x.PorterId != id))
                        throw new InvalidOperationException(string.Format(PersonnelMessages.PHONE_EXISTED, dto.Phone));
                }

                p.FullName = dto.FullName ?? p.FullName;
                p.Phone = dto.Phone ?? p.Phone;
                p.Active = dto.Active;
                p.BirthYear = dto.BirthYear;
                p.Hometown = dto.Hometown ?? p.Hometown;

                _porters.Update(p);
                return;
            }

            if (normalizedType == "vehicle")
            {
                var v = _vehicles.GetById(id)
                    ?? throw new KeyNotFoundException(string.Format(PersonnelMessages.VEHICLE_NOT_FOUND, id));

                if (!string.IsNullOrEmpty(dto.Code) && dto.Code != v.Code)
                {
                    if (_vehicles.CheckExists(x => x.Code == dto.Code && x.VehicleId != id))
                        throw new InvalidOperationException(string.Format(PersonnelMessages.CODE_EXISTED, dto.Code));
                }

                if (!string.IsNullOrEmpty(dto.PlateNumber) && dto.PlateNumber != v.PlateNumber)
                {
                    if (_vehicles.CheckExists(x => x.PlateNumber == dto.PlateNumber && x.VehicleId != id))
                        throw new InvalidOperationException(string.Format(PersonnelMessages.PLATE_EXISTED, dto.PlateNumber));
                }

                v.Code = dto.Code ?? v.Code;
                v.PlateNumber = dto.PlateNumber ?? v.PlateNumber;
                v.VehicleClass = dto.VehicleClass ?? v.VehicleClass;
                v.Active = dto.Active;
                v.MinLicenseClass = dto.MinLicenseClass ?? v.MinLicenseClass;

                if (dto.CapacityTon.HasValue)
                    v.PayloadTons = dto.CapacityTon.Value;

                _vehicles.Update(v);
                return;
            }

            throw new ArgumentException(PersonnelMessages.TYPE_INVALID);
        }

        public void Delete(string type, int id)
        {
            var normalizedType = type?.Trim().ToLowerInvariant();

            if (normalizedType == "driver")
            {
                var d = _drivers.GetById(id)
                    ?? throw new KeyNotFoundException(string.Format(PersonnelMessages.DRIVER_NOT_FOUND, id));
                _drivers.Delete(d);
                return;
            }

            if (normalizedType == "porter")
            {
                var p = _porters.GetById(id)
                    ?? throw new KeyNotFoundException(string.Format(PersonnelMessages.PORTER_NOT_FOUND, id));
                _porters.Delete(p);
                return;
            }

            if (normalizedType == "vehicle")
            {
                var v = _vehicles.GetById(id)
                    ?? throw new KeyNotFoundException(string.Format(PersonnelMessages.VEHICLE_NOT_FOUND, id));
                _vehicles.Delete(v);
                return;
            }

            throw new ArgumentException(PersonnelMessages.TYPE_INVALID);
        }

        public AvailabilityResponseDto GetAvailability(string type, DateTimeOffset at, int durationMin)
        {
            var end = at.AddMinutes(durationMin);
            var res = new AvailabilityResponseDto
            {
                Free = new List<AvailabilityItemDto>(),
                Busy = new List<AvailabilityItemDto>()
            };

            var normalizedType = type?.Trim().ToLowerInvariant();

            if (string.Equals(normalizedType, "driver", StringComparison.OrdinalIgnoreCase))
            {
                var drivers = _drivers.GetAll().Where(x => x.Active).ToList();
                foreach (var d in drivers)
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
            else if (string.Equals(normalizedType, "porter", StringComparison.OrdinalIgnoreCase))
            {
                var porters = _porters.GetAll().Where(x => x.Active).ToList();
                foreach (var p in porters)
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
            else if (string.Equals(normalizedType, "vehicle", StringComparison.OrdinalIgnoreCase))
            {
                var vehicles = _vehicles.GetAll().Where(x => x.Active).ToList();
                foreach (var v in vehicles)
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