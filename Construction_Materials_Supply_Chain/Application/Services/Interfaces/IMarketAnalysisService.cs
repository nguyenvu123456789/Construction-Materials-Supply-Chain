using Application.DTOs;

namespace Application.Interfaces
{
    public interface IMarketAnalysisService
    {
        List<MonthlyRevenueDto> GetMonthlyRevenue();
        List<TopMaterialDto> GetTopMaterials(int top = 5);
        List<SupplierRevenueDto> GetRevenueBySupplier();
        List<StaffPerformanceDto> GetRevenueByStaff();
        List<RegionRevenueDto> GetRevenueByRegion();
    }
}
