using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implements
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regions;
        private readonly IMapper _mapper;
        private readonly IValidator<RegionCreateDto> _createValidator;
        private readonly IValidator<RegionUpdateDto> _updateValidator;

        public RegionService(
            IRegionRepository regions,
            IMapper mapper,
            IValidator<RegionCreateDto> createValidator,
            IValidator<RegionUpdateDto> updateValidator)
        {
            _regions = regions;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public IEnumerable<RegionDto> GetAll()
        {
            var data = _regions.GetAll();
            return _mapper.Map<IEnumerable<RegionDto>>(data);
        }

        public RegionDto? GetById(int id)
        {
            var entity = _regions.GetById(id);
            return entity == null ? null : _mapper.Map<RegionDto>(entity);
        }

        public RegionDto Create(RegionCreateDto dto)
        {
            var vr = _createValidator.Validate(dto);
            if (!vr.IsValid) throw new ValidationException(vr.Errors);

            var nameNorm = dto.RegionName.Trim().ToLower();

            var dup = _regions
                .GetAll()
                .FirstOrDefault(r => r.RegionName.Trim().ToLower() == nameNorm);

            if (dup != null)
                throw new Exception("RegionName đã tồn tại, không thể tạo mới.");

            var entity = _mapper.Map<Region>(dto);
            _regions.Add(entity);

            return _mapper.Map<RegionDto>(entity);
        }

        public void Update(int id, RegionUpdateDto dto)
        {
            var vr = _updateValidator.Validate(dto);
            if (!vr.IsValid) throw new ValidationException(vr.Errors);

            var entity = _regions.GetById(id);
            if (entity == null) throw new KeyNotFoundException("Region not found");

            var nameNorm = dto.RegionName.Trim().ToLower();

            var dup = _regions
                .GetAll()
                .FirstOrDefault(r =>
                    r.RegionId != id &&
                    r.RegionName.Trim().ToLower() == nameNorm);

            if (dup != null)
                throw new Exception("RegionName đã tồn tại, không thể cập nhật.");

            entity.RegionName = dto.RegionName.Trim();
            _regions.Update(entity);
        }

        public void Delete(int id)
        {
            var entity = _regions.GetById(id);
            if (entity == null) return;

            if (entity.PartnerRegions != null && entity.PartnerRegions.Any())
                throw new Exception("Region đang được sử dụng bởi Partner, không thể xoá.");

            _regions.Delete(entity);
        }
    }
}
