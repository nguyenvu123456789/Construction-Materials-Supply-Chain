using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.RelationType;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;
using FluentValidation;

namespace Application.Services.Implements
{
    public class RelationTypeService : IRelationTypeService
    {
        private readonly IRelationTypeRepository _types;
        private readonly IPartnerRelationRepository _relations;
        private readonly IMapper _mapper;

        public RelationTypeService(
            IRelationTypeRepository types,
            IPartnerRelationRepository relations,
            IMapper mapper)
        {
            _types = types;
            _relations = relations;
            _mapper = mapper;
        }

        public PagedResultDto<RelationTypeDto> GetByPartner(int partnerId, int pageNumber, int pageSize)
        {
            var q = _types.QueryIncludeRelations()
                          .Where(rt => rt.PartnerRelations
                                .Any(pr => pr.BuyerPartnerId == partnerId ||
                                           pr.SellerPartnerId == partnerId));

            var total = q.Count();
            var items = q.Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

            return new PagedResultDto<RelationTypeDto>
            {
                Data = _mapper.Map<IEnumerable<RelationTypeDto>>(items),
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public void Update(int id, RelationTypeDto dto)
        {
            var entity = _types.GetById(id);
            if (entity == null)
                throw new KeyNotFoundException("RelationType not found");

            entity.Name = dto.Name;
            entity.DiscountAmount = dto.DiscountAmount;
            entity.DiscountPercent = dto.DiscountPercent;

            if (!string.IsNullOrWhiteSpace(dto.Status) &&
                dto.Status.ToLower() != "deleted")
                entity.Status = dto.Status;

            _types.Update(entity);
        }
    }
}
