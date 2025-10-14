using Application.DTOs;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IAnalystService
    {
        List<InventorySummaryDto> GetInventorySummary(ReportFilterDto filter);
        List<ConsumptionByProjectDto> GetConsumptionByProject(ReportFilterDto filter);
        List<PurchaseStatsDto> GetPurchaseStats(ReportFilterDto filter);
        List<PayableSummaryDto> GetPayables(ReportFilterDto filter);
        List<LowStockAlertDto> GetLowStockAlerts(ReportFilterDto filter);
        List<PurchaseEfficiencyDto> GetPurchaseEfficiency(ReportFilterDto filter);
        List<OverdueDebtDto> GetOverdues(ReportFilterDto filter);
        AnalyticsDashboardDto GetDashboard(ReportFilterDto filter);
        StockForecastDto ForecastStock(int materialId, int warehouseId, int days, decimal? threshold);
        List<ConsumptionForecastDto> ForecastConsumptionByProject(ReportFilterDto filter, int days);
        List<PriceTrendDto> ForecastPurchasePriceTrend(ReportFilterDto filter, int days);
        List<OverdueTrendDto> ForecastOverdueTrend(ReportFilterDto filter, int days);
    }
}
