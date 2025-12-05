using Application.Common.Pagination;
using Application.DTOs.RelationType;

namespace Application.Interfaces
{
    public interface IRelationTypeService
    {
        PagedResultDto<RelationTypeDto> GetByPartner(int partnerId, int pageNumber, int pageSize);

        void Update(int id, RelationTypeDto dto);
    }
}
