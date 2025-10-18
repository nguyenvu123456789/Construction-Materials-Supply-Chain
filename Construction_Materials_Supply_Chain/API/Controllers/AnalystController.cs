using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/analyst")]
    public class AnalystController : ControllerBase
    {
        private readonly IAnalystService _service;
        public AnalystController(IAnalystService svc) { _service = svc; }

        [HttpGet("inventory")] public ActionResult<List<InventorySummaryDto>> Inventory([FromQuery] ReportFilterDto f) => Ok(_service.GetInventorySummary(f));
        [HttpGet("consumption")] public ActionResult<List<ConsumptionByProjectDto>> Consumption([FromQuery] ReportFilterDto f) => Ok(_service.GetConsumptionByProject(f));
        [HttpGet("purchases")] public ActionResult<List<PurchaseStatsDto>> Purchases([FromQuery] ReportFilterDto f) => Ok(_service.GetPurchaseStats(f));
        [HttpGet("payables")] public ActionResult<List<PayableSummaryDto>> Payables([FromQuery] ReportFilterDto f) => Ok(_service.GetPayables(f));
        [HttpGet("low-stock")] public ActionResult<List<LowStockAlertDto>> LowStock([FromQuery] ReportFilterDto f) => Ok(_service.GetLowStockAlerts(f));
        [HttpGet("purchase-efficiency")] public ActionResult<List<PurchaseEfficiencyDto>> PurchaseEfficiency([FromQuery] ReportFilterDto f) => Ok(_service.GetPurchaseEfficiency(f));
        [HttpGet("overdues")] public ActionResult<List<OverdueDebtDto>> Overdues([FromQuery] ReportFilterDto f) => Ok(_service.GetOverdues(f));
        [HttpGet("dashboard")] public ActionResult<AnalyticsDashboardDto> Dashboard([FromQuery] ReportFilterDto f) => Ok(_service.GetDashboard(f));

        [HttpGet("forecast-stock")] public ActionResult<StockForecastDto> ForecastStock([FromQuery] int materialId, [FromQuery] int warehouseId, [FromQuery] int days = 7, [FromQuery] decimal? threshold = null) => Ok(_service.ForecastStock(materialId, warehouseId, days, threshold));
        [HttpGet("forecast-consumption")] public ActionResult<List<ConsumptionForecastDto>> ForecastConsumption([FromQuery] ReportFilterDto f, [FromQuery] int days = 7) => Ok(_service.ForecastConsumptionByProject(f, days));
        [HttpGet("forecast-price")] public ActionResult<List<PriceTrendDto>> ForecastPrice([FromQuery] ReportFilterDto f, [FromQuery] int days = 7) => Ok(_service.ForecastPurchasePriceTrend(f, days));
        [HttpGet("forecast-overdue")] public ActionResult<List<OverdueTrendDto>> ForecastOverdue([FromQuery] ReportFilterDto f, [FromQuery] int days = 7) => Ok(_service.ForecastOverdueTrend(f, days));
    }
}
