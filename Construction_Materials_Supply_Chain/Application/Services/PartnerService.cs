using Application.Common.Pagination;
using Application.DTOs.Partners;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Models;
using FluentValidation;

namespace Services.Implementations
{
    public class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _partners;
        private readonly IPartnerTypeRepository _types;
        private readonly IMapper _mapper;
        private readonly IValidator<PartnerCreateDto> _createValidator;
        private readonly IValidator<PartnerUpdateDto> _updateValidator;
        private readonly IValidator<PartnerPagedQueryDto> _filterValidator;

        public PartnerService(
            IPartnerRepository partners,
            IPartnerTypeRepository types,
            IMapper mapper,
            IValidator<PartnerCreateDto> createValidator,
            IValidator<PartnerUpdateDto> updateValidator,
            IValidator<PartnerPagedQueryDto> filterValidator)
        {
            _partners = partners;
            _types = types;
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

            var entity = _mapper.Map<Partner>(dto);
            _partners.Add(entity);
            var created = _partners.QueryWithType().FirstOrDefault(x => x.PartnerId == entity.PartnerId) ?? entity;
            return _mapper.Map<PartnerDto>(created);
        }

        public void Update(int id, PartnerUpdateDto dto)
        {
            var vr = _updateValidator.Validate(dto);
            if (!vr.IsValid) throw new ValidationException(vr.Errors);

            var entity = _partners.GetById(id);
            if (entity == null) throw new KeyNotFoundException("Partner not found");

            entity.PartnerName = dto.PartnerName;
            entity.ContactEmail = dto.ContactEmail;
            entity.ContactPhone = dto.ContactPhone;
            entity.PartnerTypeId = dto.PartnerTypeId;

            _partners.Update(entity);
        }

        public void Delete(int id)
        {
            var entity = _partners.GetById(id);
            if (entity == null) return;
            _partners.Delete(entity);
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
                    Partners = _mapper.Map<List<PartnerDto>>(map.TryGetValue(t.PartnerTypeId, out var list) ? list : new List<Partner>())
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

        public PagedResultDto<PartnerDto> GetPartnersFiltered(PartnerPagedQueryDto query)
        {
            var vr = _filterValidator.Validate(query);
            if (!vr.IsValid) throw new ValidationException(vr.Errors);

            var q = _partners.QueryWithType();

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

        public IEnumerable<PartnerTypeDto> GetPartnerTypesDto()
        {
            var types = _types.GetAll();
            return _mapper.Map<IEnumerable<PartnerTypeDto>>(types);
        }
    }
}