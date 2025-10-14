namespace Application.DTOs
{
    public class ReportFilterDto
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? WarehouseId { get; set; }
        public int? PartnerId { get; set; }
        public int? MaterialId { get; set; }
        public string? ProjectCode { get; set; }
        public decimal? LowStockThreshold { get; set; }
    }

    public class InventorySummaryDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
    }

    public class ConsumptionByProjectDto
    {
        public string ProjectCode { get; set; } = string.Empty;
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public decimal QuantityConsumed { get; set; }
    }

    public class PurchaseStatsDto
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public int OrdersCount { get; set; }
    }

    public class PayableSummaryDto
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public decimal TotalPayable { get; set; }
        public decimal OverdueAmount { get; set; }
    }

    public class LowStockAlertDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Threshold { get; set; }
    }

    public class PurchaseEfficiencyDto
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public decimal AvgUnitPrice { get; set; }
        public decimal TotalQuantity { get; set; }
        public int ImportOrders { get; set; }
    }

    public class OverdueDebtDto
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public decimal OverdueAmount { get; set; }
        public int InvoicesOverdue { get; set; }
    }

    public class AnalyticsDashboardDto
    {
        public List<InventorySummaryDto> InventorySummary { get; set; } = new();
        public List<ConsumptionByProjectDto> Consumption { get; set; } = new();
        public List<PurchaseStatsDto> Purchases { get; set; } = new();
        public List<PayableSummaryDto> Payables { get; set; } = new();
        public List<LowStockAlertDto> LowStockAlerts { get; set; } = new();
        public List<PurchaseEfficiencyDto> PurchaseEfficiency { get; set; } = new();
        public List<OverdueDebtDto> Overdues { get; set; } = new();
    }

    public class ConsumptionForecastDto
    {
        public string ProjectCode { get; set; } = string.Empty;
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public decimal AvgDailyUse { get; set; }
        public decimal ForecastUseNextDays { get; set; }
    }

    public class PriceTrendDto
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public decimal CurrentAvgPrice { get; set; }
        public decimal ForecastAvgPrice { get; set; }
        public string Trend { get; set; } = string.Empty;
    }

    public class OverdueTrendDto
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public decimal CurrentOverdue { get; set; }
        public decimal ForecastOverdue { get; set; }
        public string Trend { get; set; } = string.Empty;
    }
}
