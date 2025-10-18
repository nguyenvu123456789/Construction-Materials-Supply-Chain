using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Implements
{
    public class PersonnelService : IPersonnelService
    {
        private readonly IDriverRepository _drivers;
        private readonly IPorterRepository _porters;
        private readonly IVehicleRepository _vehicles;

        public PersonnelService(IDriverRepository drivers, IPorterRepository porters, IVehicleRepository vehicles)
        {
            _drivers = drivers;
            _porters = porters;
            _vehicles = vehicles;
        }

        public PersonResponseDto Create(PersonCreateDto dto)
        {
            var type = dto.Type?.Trim().ToLowerInvariant();
            if (type == "driver")
            {
                var d = new Driver { FullName = dto.FullName, Phone = dto.Phone, Active = dto.Active };
                _drivers.Add(d);
                return new PersonResponseDto { Type = "driver", Id = d.DriverId, FullName = d.FullName, Phone = d.Phone, Active = d.Active };
            }
            if (type == "porter")
            {
                var p = new Porter { FullName = dto.FullName, Phone = dto.Phone, Active = dto.Active };
                _porters.Add(p);
                return new PersonResponseDto { Type = "porter", Id = p.PorterId, FullName = p.FullName, Phone = p.Phone, Active = p.Active };
            }
            if (type == "vehicle")
            {
                var v = new Vehicle { Code = dto.Code!, PlateNumber = dto.PlateNumber!, VehicleClass = dto.VehicleClass, Active = dto.Active };
                _vehicles.Add(v);
                return new PersonResponseDto { Type = "vehicle", Id = v.VehicleId, FullName = v.Code, Active = v.Active, Code = v.Code, PlateNumber = v.PlateNumber, VehicleClass = v.VehicleClass };
            }
            throw new InvalidOperationException("type");
        }

        public PersonResponseDto? Get(string type, int id)
        {
            type = type.Trim().ToLowerInvariant();
            if (type == "driver")
            {
                var d = _drivers.GetById(id);
                return d == null ? null : new PersonResponseDto { Type = "driver", Id = d.DriverId, FullName = d.FullName, Phone = d.Phone, Active = d.Active };
            }
            if (type == "porter")
            {
                var p = _porters.GetById(id);
                return p == null ? null : new PersonResponseDto { Type = "porter", Id = p.PorterId, FullName = p.FullName, Phone = p.Phone, Active = p.Active };
            }
            if (type == "vehicle")
            {
                var v = _vehicles.GetById(id);
                return v == null ? null : new PersonResponseDto { Type = "vehicle", Id = v.VehicleId, FullName = v.Code, Active = v.Active, Code = v.Code, PlateNumber = v.PlateNumber, VehicleClass = v.VehicleClass };
            }
            return null;
        }

        public List<PersonResponseDto> GetAll(string type)
        {
            type = type.Trim().ToLowerInvariant();
            if (type == "driver") return _drivers.GetAll().Select(d => new PersonResponseDto { Type = "driver", Id = d.DriverId, FullName = d.FullName, Phone = d.Phone, Active = d.Active }).ToList();
            if (type == "porter") return _porters.GetAll().Select(p => new PersonResponseDto { Type = "porter", Id = p.PorterId, FullName = p.FullName, Phone = p.Phone, Active = p.Active }).ToList();
            if (type == "vehicle") return _vehicles.GetAll().Select(v => new PersonResponseDto { Type = "vehicle", Id = v.VehicleId, FullName = v.Code, Active = v.Active, Code = v.Code, PlateNumber = v.PlateNumber, VehicleClass = v.VehicleClass }).ToList();
            return new List<PersonResponseDto>();
        }

        public List<PersonResponseDto> Search(string type, string? q, bool? active, int? top)
        {
            type = type.Trim().ToLowerInvariant();
            if (type == "driver") return _drivers.Search(q, active, top).Select(d => new PersonResponseDto { Type = "driver", Id = d.DriverId, FullName = d.FullName, Phone = d.Phone, Active = d.Active }).ToList();
            if (type == "porter") return _porters.Search(q, active, top).Select(p => new PersonResponseDto { Type = "porter", Id = p.PorterId, FullName = p.FullName, Phone = p.Phone, Active = p.Active }).ToList();
            if (type == "vehicle") return _vehicles.Search(q, active, top).Select(v => new PersonResponseDto { Type = "vehicle", Id = v.VehicleId, FullName = v.Code, Active = v.Active, Code = v.Code, PlateNumber = v.PlateNumber, VehicleClass = v.VehicleClass }).ToList();
            return new List<PersonResponseDto>();
        }

        public void Update(string type, int id, PersonUpdateDto dto)
        {
            type = type.Trim().ToLowerInvariant();
            if (type == "driver")
            {
                var d = _drivers.GetById(id) ?? throw new KeyNotFoundException();
                d.FullName = dto.FullName; d.Phone = dto.Phone; d.Active = dto.Active;
                _drivers.Update(d);
                return;
            }
            if (type == "porter")
            {
                var p = _porters.GetById(id) ?? throw new KeyNotFoundException();
                p.FullName = dto.FullName; p.Phone = dto.Phone; p.Active = dto.Active;
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
                _vehicles.Update(v);
                return;
            }
            throw new InvalidOperationException("type");
        }

        public void Delete(string type, int id)
        {
            type = type.Trim().ToLowerInvariant();
            if (type == "driver") { var d = _drivers.GetById(id) ?? throw new KeyNotFoundException(); _drivers.Delete(d); return; }
            if (type == "porter") { var p = _porters.GetById(id) ?? throw new KeyNotFoundException(); _porters.Delete(p); return; }
            if (type == "vehicle") { var v = _vehicles.GetById(id) ?? throw new KeyNotFoundException(); _vehicles.Delete(v); return; }
            throw new InvalidOperationException("type");
        }
    }
}
