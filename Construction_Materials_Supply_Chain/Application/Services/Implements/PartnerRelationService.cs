using Application.DTOs.Relations;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class PartnerRelationService : IPartnerRelationService
    {
        private readonly IPartnerRelationRepository _relations;
        private readonly IMapper _mapper;

        public PartnerRelationService(IPartnerRelationRepository relations, IMapper mapper)
        {
            _relations = relations;
            _mapper = mapper;
        }

        public IEnumerable<PartnerRelationDto> GetAll()
        {
            var data = _relations.QueryWithRelations().ToList();
            return _mapper.Map<IEnumerable<PartnerRelationDto>>(data);
        }

        public PartnerRelationDto? GetById(int id)
        {
            var entity = _relations.QueryWithRelations().FirstOrDefault(x => x.PartnerRelationId == id);
            return entity == null ? null : _mapper.Map<PartnerRelationDto>(entity);
        }

        public PartnerRelationDto Create(PartnerRelationCreateDto dto)
        {
            var entity = _mapper.Map<PartnerRelation>(dto);
            _relations.Add(entity);

            var created = _relations.QueryWithRelations().First(x => x.PartnerRelationId == entity.PartnerRelationId);
            return _mapper.Map<PartnerRelationDto>(created);
        }

        public void Update(int id, PartnerRelationUpdateDto dto)
        {
            var entity = _relations.GetById(id) ?? throw new KeyNotFoundException("Relation not found");

            entity.RelationTypeId = dto.RelationTypeId;
            entity.Status = dto.Status;

            _relations.Update(entity);
        }

        public void Delete(int id)
        {
            var entity = _relations.GetById(id);
            if (entity == null) return;

            _relations.Delete(entity);
        }
        public IEnumerable<PartnerRelationDto> GetByBuyer(int buyerId)
        {
            var data = _relations.GetRelationsByBuyer(buyerId);
            return _mapper.Map<IEnumerable<PartnerRelationDto>>(data);
        }

        public IEnumerable<PartnerRelationDto> GetBySeller(int sellerId)
        {
            var data = _relations.GetRelationsBySeller(sellerId);
            return _mapper.Map<IEnumerable<PartnerRelationDto>>(data);
        }

    }
}
