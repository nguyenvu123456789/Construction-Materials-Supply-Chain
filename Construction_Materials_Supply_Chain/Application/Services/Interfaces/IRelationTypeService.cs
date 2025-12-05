using Application.Common.Pagination;
using Application.DTOs.RelationType;

namespace Application.Interfaces
{
    public interface IRelationTypeService
    {
        PagedResultDto<RelationTypeDto> GetByPartner(int partnerId, int pageNumber, int pageSize);
        Task<RelationTypeDto> Create(CreateRelationTypeDto dto);
        void Update(int id, RelationTypeDto dto);
    }
}
