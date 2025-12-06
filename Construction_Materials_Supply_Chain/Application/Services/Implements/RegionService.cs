using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using FluentValidation;
using System.Text.Json;

namespace Application.Services.Implements
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regions;
        private readonly IMapper _mapper;
        private readonly IValidator<RegionCreateDto> _createValidator;
        private readonly IValidator<RegionUpdateDto> _updateValidator;
        private readonly Dictionary<string, HashSet<string>> _map = new();

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
            LoadVietnamMap();
        }

        private void LoadVietnamMap()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "vietnam.json");
            var json = File.ReadAllText(path);
            var provinces = JsonSerializer.Deserialize<List<ProvinceDto>>(json) ?? new List<ProvinceDto>();
            foreach (var p in provinces)
            {
                if (string.IsNullOrWhiteSpace(p.Name)) continue;
                var provinceNormalized = Normalize(p.Name);
                var districts = (p.Wards ?? new List<WardDto>())
                    .Select(w => Normalize(w.Name))
                    .ToHashSet();
                _map[provinceNormalized] = districts;
            }
        }

        public bool CanTrade(string regionA, string regionB)
        {
            var a = Normalize(regionA);
            var b = Normalize(regionB);
            foreach (var province in _map.Keys)
            {
                var districts = _map[province];
                if (districts.Contains(a) && province == b) return true;
                if (districts.Contains(b) && province == a) return true;
                if (districts.Contains(a) && districts.Contains(b)) return true;
                if (province == a && province == b) return true;
            }
            return false;
        }

        public bool RegionExists(string regionName)
        {
            var normalized = Normalize(regionName);
            if (_map.ContainsKey(normalized)) return true;
            foreach (var province in _map.Keys)
            {
                if (_map[province].Contains(normalized)) return true;
            }
            return false;
        }

        private static string Normalize(string s)
        {
            return s.Trim().ToLower()
                .Replace("quận", "")
                .Replace("huyện", "")
                .Replace("thành phố", "")
                .Replace("tp.", "")
                .Replace("tp", "")
                .Trim();
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
            var dup = _regions.GetAll()
                .FirstOrDefault(r => r.RegionName.Trim().ToLower() == nameNorm);
            if (dup != null) throw new Exception("RegionName đã tồn tại, không thể tạo mới.");
            if (!RegionExists(dto.RegionName))
                throw new Exception($"RegionName '{dto.RegionName}' không tìm thấy trong bản đồ Vietnam.");
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
            var dup = _regions.GetAll()
                .FirstOrDefault(r => r.RegionId != id && r.RegionName.Trim().ToLower() == nameNorm);
            if (dup != null) throw new Exception("RegionName đã tồn tại, không thể cập nhật.");
            if (!RegionExists(dto.RegionName))
                throw new Exception($"RegionName '{dto.RegionName}' không tìm thấy trong bản đồ Vietnam.");
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
