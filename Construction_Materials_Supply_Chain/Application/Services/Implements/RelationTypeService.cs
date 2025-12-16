using Application.Common.Pagination;
using Application.Constants.Enums;
using Application.Constants.Messages;
using Application.DTOs.RelationType;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Models;
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
        public async Task<RelationTypeDto> Create(CreateRelationTypeDto dto)
        {
            // Check trùng tên
            var exists = _types.Query()
                               .Any(rt => rt.Name.ToLower() == dto.Name.ToLower());
            if (exists)
                throw new Exception(RelationTypeMessages.NAME_EXISTS);

            var entity = new RelationType
            {
                Name = dto.Name,
                DiscountAmount = dto.DiscountAmount,
                DiscountPercent = dto.DiscountPercent,
                Status = dto.Status
            };

            _types.Add(entity);

            return await Task.FromResult(_mapper.Map<RelationTypeDto>(entity));
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
                throw new KeyNotFoundException(RelationTypeMessages.NOT_FOUND);

            entity.Name = dto.Name;
            entity.DiscountAmount = dto.DiscountAmount;
            entity.DiscountPercent = dto.DiscountPercent;

            if (!string.IsNullOrWhiteSpace(dto.Status) &&
                dto.Status.ToLower() != StatusEnum.Deleted.ToStatusString())
                entity.Status = dto.Status;

            _types.Update(entity);
        }
    }
}
