using Application.Common.Pagination;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Interfaces;
using Domain.Models;
using FluentValidation;
using System.Globalization;
using System.Text;

namespace Application.Services.Implements
{
    public class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _partners;
        private readonly IPartnerTypeRepository _types;
        private readonly IMapper _mapper;
        private readonly IValidator<PartnerCreateDto> _createValidator;
        private readonly IValidator<PartnerUpdateDto> _updateValidator;
        private readonly IValidator<PartnerPagedQueryDto> _filterValidator;
        private readonly IRegionRepository _regions;

        public PartnerService(
            IPartnerRepository partners,
            IPartnerTypeRepository types,
            IRegionRepository regions,
            IMapper mapper,
            IValidator<PartnerCreateDto> createValidator,
            IValidator<PartnerUpdateDto> updateValidator,
            IValidator<PartnerPagedQueryDto> filterValidator)
        {
            _partners = partners;
            _types = types;
            _regions = regions;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _filterValidator = filterValidator;
        }

        public IEnumerable<PartnerDto> GetAllDto()
        {
            var data = _partners.QueryWithType().ToList();
            return _mapper.Map<IEnumerable<PartnerDto>>(data);
        }

        public PartnerDto? GetDto(int id)
        {
            var entity = _partners.QueryWithType().FirstOrDefault(x => x.PartnerId == id);
            return entity == null ? null : _mapper.Map<PartnerDto>(entity);
        }

        public PartnerDto Create(PartnerCreateDto dto)
        {
            var vr = _createValidator.Validate(dto);
            if (!vr.IsValid) throw new ValidationException(vr.Errors);

            string name = dto.PartnerName.Trim().ToLower();
            string code = dto.PartnerCode.Trim().ToLower();
            string? email = dto.ContactEmail?.Trim().ToLower();
            string? phone = dto.ContactPhone?.Trim().ToLower();

            var duplicate = _partners.QueryWithTypeIncludeDeleted()
                .FirstOrDefault(p =>
                    (!string.IsNullOrEmpty(p.PartnerName) &&
                     p.PartnerName.Trim().ToLower() == name)
                    || (!string.IsNullOrEmpty(p.PartnerCode) &&
                        p.PartnerCode.Trim().ToLower() == code)
                    || (!string.IsNullOrEmpty(email) &&
                        !string.IsNullOrEmpty(p.ContactEmail) &&
                        p.ContactEmail.Trim().ToLower() == email)
                    || (!string.IsNullOrEmpty(phone) &&
                        !string.IsNullOrEmpty(p.ContactPhone) &&
                        p.ContactPhone.Trim().ToLower() == phone)
                );

            if (duplicate != null)
                throw new Exception("Partner bị trùng dữ liệu (Tên / Code / Email / Phone). Không thể tạo mới.");

            var entity = _mapper.Map<Partner>(dto);

            if (dto.RegionNames != null && dto.RegionNames.Any())
            {
                var regionIds = ResolveRegionIdsFromNames(dto.RegionNames);

                foreach (var regionId in regionIds.Distinct())
                {
                    entity.PartnerRegions.Add(new PartnerRegion
                    {
                        RegionId = regionId
                    });
                }
            }

            _partners.Add(entity);

            var created = _partners.QueryWithTypeIncludeDeleted()
                .FirstOrDefault(x => x.PartnerId == entity.PartnerId) ?? entity;

            return _mapper.Map<PartnerDto>(created);
        }

        public void Update(int id, PartnerUpdateDto dto)
        {
            var vr = _updateValidator.Validate(dto);
            if (!vr.IsValid) throw new ValidationException(vr.Errors);

            string name = dto.PartnerName.Trim().ToLower();
            string? email = dto.ContactEmail?.Trim().ToLower();
            string? phone = dto.ContactPhone?.Trim().ToLower();

            var duplicate = _partners.QueryWithTypeIncludeDeleted()
                .FirstOrDefault(p =>
                    p.PartnerId != id && (
                        (!string.IsNullOrEmpty(p.PartnerName) &&
                         p.PartnerName.Trim().ToLower() == name)
                        || (!string.IsNullOrEmpty(email) &&
                            !string.IsNullOrEmpty(p.ContactEmail) &&
                            p.ContactEmail.Trim().ToLower() == email)
                        || (!string.IsNullOrEmpty(phone) &&
                            !string.IsNullOrEmpty(p.ContactPhone) &&
                            p.ContactPhone.Trim().ToLower() == phone)
                    ));

            if (duplicate != null)
                throw new Exception("Partner bị trùng dữ liệu (Tên / Email / Phone). Không thể cập nhật.");

            var entity = _partners.QueryWithTypeIncludeDeleted()
                                  .FirstOrDefault(x => x.PartnerId == id);
            if (entity == null) throw new KeyNotFoundException("Partner not found");
            if (entity.Status == "Deleted") throw new InvalidOperationException("Cannot update deleted partner");

            entity.PartnerName = dto.PartnerName;
            entity.ContactEmail = dto.ContactEmail;
            entity.ContactPhone = dto.ContactPhone;
            entity.PartnerTypeId = dto.PartnerTypeId;

            if (!string.IsNullOrWhiteSpace(dto.Status) &&
                !string.Equals(dto.Status, "Deleted", StringComparison.OrdinalIgnoreCase))
            {
                var s = dto.Status.Trim();
                entity.Status = char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
            }

            if (dto.RegionNames != null && dto.RegionNames.Any())
            {
                var regionIds = ResolveRegionIdsFromNames(dto.RegionNames);

                entity.PartnerRegions.Clear();

                foreach (var regionId in regionIds.Distinct())
                {
                    entity.PartnerRegions.Add(new PartnerRegion
                    {
                        PartnerId = entity.PartnerId,
                        RegionId = regionId
                    });
                }
            }

            _partners.Update(entity);
        }

        private List<int> ResolveRegionIdsFromNames(IEnumerable<string>? regionNames)
        {
            var result = new List<int>();
            if (regionNames == null) return result;

            var allRegions = _regions.GetAll().ToList();

            foreach (var rawGroup in regionNames)
            {
                if (string.IsNullOrWhiteSpace(rawGroup)) continue;

                var pieces = rawGroup
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrWhiteSpace(p));

                foreach (var raw in pieces)
                {
                    var key = raw.Trim().ToLower();
                    if (string.IsNullOrEmpty(key)) continue;

                    var existing = allRegions.FirstOrDefault(r =>
                        !string.IsNullOrEmpty(r.RegionName) &&
                        r.RegionName.Trim().ToLower() == key
                    );

                    if (existing == null)
                    {
                        existing = new Region
                        {
                            RegionName = raw.Trim()
                        };

                        _regions.Add(existing);
                        allRegions.Add(existing);
                    }

                    if (!result.Contains(existing.RegionId))
                        result.Add(existing.RegionId);
                }
            }

            return result;
        }

        public void Delete(int id)
        {
            var entity = _partners.GetById(id);
            if (entity == null) return;
            if (entity.Status == "Deleted") return;
            _partners.SoftDelete(entity);
        }

        public void Restore(int id, string status)
        {
            var s = (status ?? string.Empty).Trim();
            if (!string.Equals(s, "Active", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(s, "Inactive", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Status must be Active or Inactive");

            var p = _partners.QueryWithTypeIncludeDeleted().FirstOrDefault(x => x.PartnerId == id);
            if (p == null) throw new KeyNotFoundException("Partner not found");
            if (p.Status != "Deleted") return;
            p.Status = char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
            _partners.Update(p);
        }

        public IEnumerable<PartnerTypeDto> GetPartnerTypesWithPartnersDto()
        {
            var types = _types.GetAll();
            var partners = _partners.QueryWithType().ToList();

            var map = types.ToDictionary(t => t.PartnerTypeId, t => new List<Partner>());
            foreach (var p in partners)
            {
                if (map.ContainsKey(p.PartnerTypeId)) map[p.PartnerTypeId].Add(p);
            }

            var result = new List<PartnerTypeDto>();
            foreach (var t in types)
            {
                var dto = new PartnerTypeDto
                {
                    PartnerTypeId = t.PartnerTypeId,
                    TypeName = t.TypeName,
                    Partners = _mapper.Map<List<PartnerDto>>(
                        map.TryGetValue(t.PartnerTypeId, out var list) ? list : new List<Partner>())
                };
                result.Add(dto);
            }
            return result;
        }

        public IEnumerable<PartnerDto> GetPartnersByTypeDto(int partnerTypeId)
        {
            var items = _partners.QueryWithType().Where(p => p.PartnerTypeId == partnerTypeId).ToList();
            return _mapper.Map<IEnumerable<PartnerDto>>(items);
        }

        public PagedResultDto<PartnerDto> GetPartnersFiltered(PartnerPagedQueryDto query, List<string>? statuses = null)
        {
            var vr = _filterValidator.Validate(query);
            if (!vr.IsValid) throw new ValidationException(vr.Errors);

            var q = _partners.QueryWithType();

            if (statuses != null && statuses.Count > 0)
            {
                var set = new HashSet<string>(statuses.Where(s => !string.IsNullOrWhiteSpace(s))
                                                      .Select(s => s.Trim().ToLowerInvariant()));
                set.Remove("deleted");
                if (set.Count > 0)
                    q = q.Where(p => set.Contains(p.Status.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                q = q.Where(p => (p.PartnerName ?? string.Empty).Contains(query.SearchTerm));

            if (query.PartnerTypes != null && query.PartnerTypes.Any())
            {
                var typeIds = _types.GetAll()
                    .Where(t => query.PartnerTypes.Contains(t.TypeName))
                    .Select(t => t.PartnerTypeId)
                    .ToHashSet();
                q = q.Where(p => typeIds.Contains(p.PartnerTypeId));
            }

            var total = q.Count();
            if (query.PageNumber > 0 && query.PageSize > 0)
                q = q.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);

            var items = _mapper.Map<IEnumerable<PartnerDto>>(q.ToList());

            return new PagedResultDto<PartnerDto>
            {
                Data = items,
                TotalCount = total,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(total / (double)query.PageSize)
            };
        }

        public PagedResultDto<PartnerDto> GetPartnersFilteredIncludeDeleted(PartnerPagedQueryDto query, List<string>? statuses = null)
        {
            var vr = _filterValidator.Validate(query);
            if (!vr.IsValid) throw new ValidationException(vr.Errors);

            var q = _partners.QueryWithTypeIncludeDeleted();

            if (statuses != null && statuses.Count > 0)
            {
                var set = new HashSet<string>(statuses.Where(s => !string.IsNullOrWhiteSpace(s))
                                                      .Select(s => s.Trim().ToLowerInvariant()));
                q = q.Where(p => set.Contains(p.Status.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                q = q.Where(p => (p.PartnerName ?? string.Empty).Contains(query.SearchTerm));

            if (query.PartnerTypes != null && query.PartnerTypes.Any())
            {
                var typeIds = _types.GetAll()
                    .Where(t => query.PartnerTypes.Contains(t.TypeName))
                    .Select(t => t.PartnerTypeId)
                    .ToHashSet();
                q = q.Where(p => typeIds.Contains(p.PartnerTypeId));
            }

            var total = q.Count();
            if (query.PageNumber > 0 && query.PageSize > 0)
                q = q.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);

            var items = _mapper.Map<IEnumerable<PartnerDto>>(q.ToList());

            return new PagedResultDto<PartnerDto>
            {
                Data = items,
                TotalCount = total,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public IEnumerable<PartnerTypeDto> GetPartnerTypesDto()
        {
            var types = _types.GetAll();
            return _mapper.Map<IEnumerable<PartnerTypeDto>>(types);
        }
    }
}
