using Application.Common.Pagination;
using Application.DTOs.Application.DTOs;

namespace Application.Services.Interfaces
{
    public interface IPriceMaterialPartnerService
    {
        Task<PagedResultDto<PriceMaterialPartnerDto>> GetAllAsync(PriceCatalogQueryDto query);
        Task UpdatePriceAsync(PriceMaterialPartnerUpdateDto dto);
        Task<List<PriceMaterialPartnerDto>> GetPricesForPartnerAsync(int buyerPartnerId, int sellerPartnerId);


    }
}
