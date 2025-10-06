using Application.Common.Pagination;
using Application.DTOs.Partners;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IPartnerService
    {
        IEnumerable<PartnerDto> GetAllDto();
        PartnerDto? GetDto(int id);
        PartnerDto Create(PartnerCreateDto dto);
        void Update(int id, PartnerUpdateDto dto);
        void Delete(int id);
        IEnumerable<PartnerTypeDto> GetPartnerTypesWithPartnersDto();
        IEnumerable<PartnerDto> GetPartnersByTypeDto(int partnerTypeId);
        PagedResultDto<PartnerDto> GetPartnersFiltered(PartnerPagedQueryDto query);
        IEnumerable<PartnerTypeDto> GetPartnerTypesDto();
    }
}