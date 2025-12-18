using Application.Constants.Messages;
using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Interfaces;
using Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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
        private readonly IOrderRepository _orders;
        private readonly IPartnerRegionRepository _partnerRegions;


        public RegionService(
            IRegionRepository regions,
            IOrderRepository orders,
            IPartnerRegionRepository partnerRegions,
            IMapper mapper,
            IValidator<RegionCreateDto> createValidator,
            IValidator<RegionUpdateDto> updateValidator)
        {
            _regions = regions;
            _orders = orders;
            _partnerRegions = partnerRegions;
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
            if (dup != null)
                throw new Exception(RegionMessages.MSG_REGION_NAME_EXISTS);
            if (!RegionExists(dto.RegionName))
                throw new Exception(string.Format(RegionMessages.MSG_REGION_NOT_IN_VIETNAM_MAP, dto.RegionName));
            var entity = _mapper.Map<Region>(dto);
            _regions.Add(entity);
            return _mapper.Map<RegionDto>(entity);
        }
        public void Update(int id, RegionUpdateDto dto)
        {
            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var entity = _regions.GetById(id);
            if (entity == null)
                throw new KeyNotFoundException(RegionMessages.MSG_REGION_NOT_FOUND);

            var nameNorm = dto.RegionName.Trim().ToLower();
            var duplicate = _regions.GetAll()
                .FirstOrDefault(r => r.RegionId != id && r.RegionName.Trim().ToLower() == nameNorm);

            if (duplicate != null)
                throw new Exception(RegionMessages.MSG_REGION_NAME_EXISTS);

            if (!RegionExists(dto.RegionName))
                throw new Exception(string.Format(RegionMessages.MSG_REGION_NOT_IN_VIETNAM_MAP, dto.RegionName));

            entity.RegionName = dto.RegionName.Trim();
            _regions.Update(entity);
        }

        public void Delete(int id)
        {
            var entity = _regions.GetById(id);
            if (entity == null)
                return;

            if (entity.PartnerRegions != null && entity.PartnerRegions.Any())
                throw new Exception(RegionMessages.MSG_REGION_IN_USE_BY_PARTNER);

            _regions.Delete(entity);
        }

        public IEnumerable<PartnerWithRegionsDto> GetBuyerRegions(int partnerId)
        {
            // Lấy các partner là người bán liên quan đến partnerId (người mua)
            var sellerPartnerIds = _orders.GetSellerPartnerIds(partnerId);
            if (!sellerPartnerIds.Any())
                return Enumerable.Empty<PartnerWithRegionsDto>();

            // Lấy partner cùng region
            var partners = _partnerRegions.GetPartnersWithRegionsByIds(sellerPartnerIds);

            // Map ra DTO theo partner -> region
            var result = partners.Select(p => new PartnerWithRegionsDto
            {
                PartnerId = p.PartnerId,
                PartnerName = p.PartnerName,
                Regions = p.PartnerRegions
                    .Select(pr => new RegionDto
                    {
                        RegionId = pr.Region.RegionId,
                        RegionName = pr.Region.RegionName
                    })
                    .ToList()
            });

            return result;
        }

        public IEnumerable<PartnerWithRegionsDto> GetSellerRegions(int partnerId)
        {
            // Lấy các partner là người mua liên quan đến partnerId (người bán)
            var buyerPartnerIds = _orders.GetBuyerPartnerIds(partnerId);
            if (!buyerPartnerIds.Any())
                return Enumerable.Empty<PartnerWithRegionsDto>();

            var partners = _partnerRegions.GetPartnersWithRegionsByIds(buyerPartnerIds);

            var result = partners.Select(p => new PartnerWithRegionsDto
            {
                PartnerId = p.PartnerId,
                PartnerName = p.PartnerName,
                Regions = p.PartnerRegions
                    .Select(pr => new RegionDto
                    {
                        RegionId = pr.Region.RegionId,
                        RegionName = pr.Region.RegionName
                    })
                    .ToList()
            });

            return result;
        }


    }
}
