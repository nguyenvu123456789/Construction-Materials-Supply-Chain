using Application.Common.Pagination;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IPartnerService
    {
        IEnumerable<PartnerDto> GetAllDto();
        PartnerDto? GetDto(int id);
        PartnerDto Create(PartnerCreateDto dto);
        void Update(int id, PartnerUpdateDto dto);
        void Delete(int id);
        void Restore(int id, string status);
        IEnumerable<PartnerTypeDto> GetPartnerTypesWithPartnersDto();
        IEnumerable<PartnerDto> GetPartnersByTypeDto(int partnerTypeId);
        PagedResultDto<PartnerDto> GetPartnersFiltered(PartnerPagedQueryDto query, List<string>? statuses = null);
        PagedResultDto<PartnerDto> GetPartnersFilteredIncludeDeleted(PartnerPagedQueryDto query, List<string>? statuses = null);
        IEnumerable<PartnerTypeDto> GetPartnerTypesDto();
    }
}
