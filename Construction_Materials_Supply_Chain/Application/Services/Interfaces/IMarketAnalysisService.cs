using Application.DTOs;
using Application.DTOs.Application.DTOs;

namespace Application.Interfaces
{
    public interface IMarketAnalysisService
    {
        List<TopMaterialDto> GetTopMaterials(int top = 5);
        List<SupplierRevenueDto> GetRevenueBySupplier();
        List<WeeklyRevenueDto> GetWeeklyRevenueByPartner(int userId);
    }
}
