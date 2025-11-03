using Application.Common.Pagination;
using Application.DTOs.Application.DTOs;
using Application.DTOs.Common.Pagination;

namespace Application.Services.Interfaces
{
    public interface IPriceMaterialPartnerService
    {
        PagedResultDto<PriceMaterialPartnerDto> GetAll(PriceCatalogQueryDto query);
        void UpdatePrice(PriceMaterialPartnerUpdateDto dto);
    }
}
