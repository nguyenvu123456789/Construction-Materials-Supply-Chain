using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;

namespace Services.Implementations
{
    public class AnalystService : IAnalystService
    {
        private readonly IAnalystRepository _repo;
        public AnalystService(IAnalystRepository repo) { _repo = repo; }

        public List<InventorySummaryDto> GetInventorySummary(ReportFilterDto f)
        {
            var q = _repo.InventoriesWithWarehouseMaterial();
            if (f.WarehouseId.HasValue) q = q.Where(x => x.WarehouseId == f.WarehouseId.Value);
            if (f.MaterialId.HasValue) q = q.Where(x => x.MaterialId == f.MaterialId.Value);
            return q.Select(x => new InventorySummaryDto
            {
                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse.WarehouseName,
                MaterialId = x.MaterialId,
                MaterialName = x.Material.MaterialName,
                Quantity = (decimal)x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList();
        }

        public List<ConsumptionByProjectDto> GetConsumptionByProject(ReportFilterDto f)
        {
            var q = _repo.ExportDetailsWithExport();
            if (f.From.HasValue) q = q.Where(x => x.Export.CreatedAt >= f.From.Value);
            if (f.To.HasValue) q = q.Where(x => x.Export.CreatedAt <= f.To.Value);
            if (f.MaterialId.HasValue) q = q.Where(x => x.MaterialId == f.MaterialId.Value);
            var code = f.ProjectCode ?? "";
            if (!string.IsNullOrWhiteSpace(code)) q = q.Where(x => (x.Export.ExportCode ?? "").Contains(code));
            return q.GroupBy(x => new { Code = x.Export.ExportCode, x.MaterialId, x.MaterialName })
                .Select(g => new ConsumptionByProjectDto
                {
                    ProjectCode = g.Key.Code ?? "",
                    MaterialId = g.Key.MaterialId,
                    MaterialName = g.Key.MaterialName,
                    QuantityConsumed = g.Sum(i => i.Quantity)
                }).ToList();
        }

        public List<PurchaseStatsDto> GetPurchaseStats(ReportFilterDto f)
        {
            var q = _repo.InvoiceDetailsWithInvoicePartner().Where(d => d.Invoice.InvoiceType == "Purchase");
            if (f.From.HasValue) q = q.Where(x => x.Invoice.CreatedAt >= f.From.Value);
            if (f.To.HasValue) q = q.Where(x => x.Invoice.CreatedAt <= f.To.Value);
            if (f.PartnerId.HasValue) q = q.Where(x => x.Invoice.PartnerId == f.PartnerId.Value);
            return q.GroupBy(x => new { x.Invoice.PartnerId, x.Invoice.Partner.PartnerName })
                .Select(g => new PurchaseStatsDto
                {
                    PartnerId = g.Key.PartnerId,
                    PartnerName = g.Key.PartnerName ?? "",
                    TotalQuantity = g.Sum(i => (decimal)i.Quantity),
                    TotalAmount = g.Sum(i => (decimal)(i.LineTotal ?? 0)),
                    OrdersCount = g.Select(i => i.InvoiceId).Distinct().Count()
                }).ToList();
        }

        public List<PayableSummaryDto> GetPayables(ReportFilterDto f)
        {
            var q = _repo.PurchaseInvoicesWithPartner();
            if (f.From.HasValue) q = q.Where(i => i.CreatedAt >= f.From.Value);
            if (f.To.HasValue) q = q.Where(i => i.CreatedAt <= f.To.Value);
            if (f.PartnerId.HasValue) q = q.Where(i => i.PartnerId == f.PartnerId.Value);
            return q.GroupBy(i => new { i.PartnerId, Name = i.Partner.PartnerName })
                .Select(g => new PayableSummaryDto
                {
                    PartnerId = g.Key.PartnerId,
                    PartnerName = g.Key.Name ?? "",
                    TotalPayable = g.Sum(x => x.TotalAmount),
                    OverdueAmount = g.Where(x => x.Status == "Overdue").Sum(x => x.TotalAmount)
                }).ToList();
        }

        public List<LowStockAlertDto> GetLowStockAlerts(ReportFilterDto f)
        {
            var threshold = f.LowStockThreshold ?? 0;
            var q = _repo.InventoriesWithWarehouseMaterial();
            if (f.WarehouseId.HasValue) q = q.Where(x => x.WarehouseId == f.WarehouseId.Value);
            if (f.MaterialId.HasValue) q = q.Where(x => x.MaterialId == f.MaterialId.Value);
            return q.Where(x => x.Quantity <= threshold)
                .Select(x => new LowStockAlertDto
                {
                    WarehouseId = x.WarehouseId,
                    WarehouseName = x.Warehouse.WarehouseName,
                    MaterialId = x.MaterialId,
                    MaterialName = x.Material.MaterialName,
                    Quantity = (decimal)x.Quantity,
                    Threshold = threshold
                }).ToList();
        }

        public List<PurchaseEfficiencyDto> GetPurchaseEfficiency(ReportFilterDto f)
        {
            var q = _repo.InvoiceDetailsWithInvoicePartner().Where(d => d.Invoice.InvoiceType == "Purchase");
            if (f.From.HasValue) q = q.Where(x => x.Invoice.CreatedAt >= f.From.Value);
            if (f.To.HasValue) q = q.Where(x => x.Invoice.CreatedAt <= f.To.Value);
            return q.GroupBy(x => new { x.Invoice.PartnerId, x.Invoice.Partner.PartnerName })
                .Select(g => new PurchaseEfficiencyDto
                {
                    PartnerId = g.Key.PartnerId,
                    PartnerName = g.Key.PartnerName ?? "",
                    AvgUnitPrice = g.Sum(i => i.UnitPrice * i.Quantity) / (g.Sum(i => i.Quantity) == 0 ? 1 : g.Sum(i => i.Quantity)),
                    TotalQuantity = g.Sum(i => (decimal)i.Quantity),
                    ImportOrders = g.Select(i => i.InvoiceId).Distinct().Count()
                }).ToList();
        }

        public List<OverdueDebtDto> GetOverdues(ReportFilterDto f)
        {
            var q = _repo.PurchaseInvoicesWithPartner().Where(i => i.Status == "Overdue");
            if (f.From.HasValue) q = q.Where(i => i.CreatedAt >= f.From.Value);
            if (f.To.HasValue) q = q.Where(i => i.CreatedAt <= f.To.Value);
            if (f.PartnerId.HasValue) q = q.Where(i => i.PartnerId == f.PartnerId.Value);
            return q.GroupBy(i => new { i.PartnerId, Name = i.Partner.PartnerName })
                .Select(g => new OverdueDebtDto
                {
                    PartnerId = g.Key.PartnerId,
                    PartnerName = g.Key.Name ?? "",
                    OverdueAmount = g.Sum(x => x.TotalAmount),
                    InvoicesOverdue = g.Count()
                }).ToList();
        }

        public AnalyticsDashboardDto GetDashboard(ReportFilterDto f)
        {
            return new AnalyticsDashboardDto
            {
                InventorySummary = GetInventorySummary(f),
                Consumption = GetConsumptionByProject(f),
                Purchases = GetPurchaseStats(f),
                Payables = GetPayables(f),
                LowStockAlerts = GetLowStockAlerts(f),
                PurchaseEfficiency = GetPurchaseEfficiency(f),
                Overdues = GetOverdues(f)
            };
        }

        public StockForecastDto ForecastStock(int materialId, int warehouseId, int days, decimal? threshold)
        {
            var hist = _repo.InventoriesWithWarehouseMaterial()
                .Where(i => i.MaterialId == materialId && i.WarehouseId == warehouseId)
                .OrderBy(i => i.CreatedAt)
                .Select(i => new { Date = i.CreatedAt ?? DateTime.UtcNow, i.Quantity })
                .ToList();
            if (hist.Count < 3) return new StockForecastDto { MaterialId = materialId, WarehouseId = warehouseId, ForecastStatus = "Insufficient Data" };

            var xs = Enumerable.Range(0, hist.Count).Select(i => (double)i).ToArray();
            var ys = hist.Select(i => (double)i.Quantity).ToArray();
            double xAvg = xs.Average();
            double yAvg = ys.Average();
            double slope = xs.Zip(ys, (x, y) => (x - xAvg) * (y - yAvg)).Sum() / xs.Sum(x => Math.Pow(x - xAvg, 2));
            double intercept = yAvg - slope * xAvg;
            double nextX = hist.Count + days;
            double predicted = Math.Max(0, slope * nextX + intercept);

            var current = (decimal)ys.Last();
            var forecast = (decimal)predicted;
            var status = (threshold.HasValue && forecast < threshold.Value) ? "Low Stock Warning" : "Normal";
            var trend = slope < 0 ? "Decreasing" : "Increasing";

            return new StockForecastDto
            {
                MaterialId = materialId,
                WarehouseId = warehouseId,
                CurrentQuantity = current,
                ForecastQuantity = forecast,
                Trend = trend,
                ForecastStatus = status
            };
        }

        public List<ConsumptionForecastDto> ForecastConsumptionByProject(ReportFilterDto f, int days)
        {
            var q = _repo.ExportDetailsWithExport();

            if (f.From.HasValue) q = q.Where(x => x.Export.ExportDate >= f.From.Value);
            if (f.To.HasValue) q = q.Where(x => x.Export.ExportDate <= f.To.Value);
            q = q.Where(x => x.Export.Status == "Approved" || x.Export.Status == "Success");

            if (f.MaterialId.HasValue) q = q.Where(x => x.MaterialId == f.MaterialId.Value);
            if (!string.IsNullOrWhiteSpace(f.ProjectCode))
                q = q.Where(x => x.Export.ExportCode == f.ProjectCode);

            var daily = q.GroupBy(x => new { Day = x.Export.ExportDate.Date, x.Export.ExportCode, x.MaterialId, x.MaterialName })
                .Select(g => new { g.Key.ExportCode, g.Key.MaterialId, g.Key.MaterialName, Qty = g.Sum(i => i.Quantity), Day = g.Key.Day })
                .OrderBy(x => x.Day)
                .ToList();

            var result = new List<ConsumptionForecastDto>();

            foreach (var grp in daily.GroupBy(x => new { x.ExportCode, x.MaterialId, x.MaterialName }))
            {
                var s = grp.OrderBy(x => x.Day).ToList();
                if (s.Count < 1) continue;

                var xs = Enumerable.Range(0, s.Count).Select(i => (double)i).ToArray();
                var ys = s.Select(i => (double)i.Qty).ToArray();

                double xAvg = xs.Average(), yAvg = ys.Average();
                double denom = xs.Sum(x => Math.Pow(x - xAvg, 2));
                double slope = denom == 0 ? 0 : xs.Zip(ys, (x, y) => (x - xAvg) * (y - yAvg)).Sum() / denom;
                double intercept = yAvg - slope * xAvg;

                double forecastTotal = 0;
                for (int k = 1; k <= days; k++)
                {
                    double yk = Math.Max(0, slope * (s.Count - 1 + k) + intercept);
                    forecastTotal += yk;
                }

                result.Add(new ConsumptionForecastDto
                {
                    ProjectCode = grp.Key.ExportCode ?? "",
                    MaterialId = grp.Key.MaterialId,
                    MaterialName = grp.Key.MaterialName,
                    AvgDailyUse = (decimal)ys.Average(),
                    ForecastUseNextDays = (decimal)forecastTotal
                });
            }
            return result;
        }

        public List<PriceTrendDto> ForecastPurchasePriceTrend(ReportFilterDto f, int days)
        {
            var q = _repo.InvoiceDetailsWithInvoicePartner().Where(d => d.Invoice.InvoiceType == "Purchase");
            if (f.From.HasValue) q = q.Where(x => x.Invoice.CreatedAt >= f.From.Value);
            if (f.To.HasValue) q = q.Where(x => x.Invoice.CreatedAt <= f.To.Value);
            if (f.PartnerId.HasValue) q = q.Where(x => x.Invoice.PartnerId == f.PartnerId.Value);

            var perDay = q.GroupBy(d => new { d.Invoice.PartnerId, d.Invoice.Partner.PartnerName, Day = d.Invoice.CreatedAt!.Value.Date })
                .Select(g => new
                {
                    g.Key.PartnerId,
                    g.Key.PartnerName,
                    Day = g.Key.Day,
                    AvgPrice = g.Sum(x => x.UnitPrice * x.Quantity) / (g.Sum(x => x.Quantity) == 0 ? 1 : g.Sum(x => x.Quantity))
                })
                .OrderBy(x => x.Day)
                .ToList();

            var result = new List<PriceTrendDto>();
            foreach (var grp in perDay.GroupBy(x => new { x.PartnerId, x.PartnerName }))
            {
                var s = grp.OrderBy(x => x.Day).ToList();
                if (s.Count < 1) continue;
                var xs = Enumerable.Range(0, s.Count).Select(i => (double)i).ToArray();
                var ys = s.Select(i => (double)i.AvgPrice).ToArray();
                double xAvg = xs.Average();
                double yAvg = ys.Average();
                double slope = xs.Zip(ys, (x, y) => (x - xAvg) * (y - yAvg)).Sum() / xs.Sum(x => Math.Pow(x - xAvg, 2));
                double intercept = yAvg - slope * xAvg;
                double nextX = s.Count + days;
                double predicted = Math.Max(0, slope * nextX + intercept);

                result.Add(new PriceTrendDto
                {
                    PartnerId = grp.Key.PartnerId,
                    PartnerName = grp.Key.PartnerName ?? "",
                    CurrentAvgPrice = (decimal)ys.Last(),
                    ForecastAvgPrice = (decimal)predicted,
                    Trend = slope < 0 ? "Decreasing" : "Increasing"
                });
            }
            return result;
        }

        public List<OverdueTrendDto> ForecastOverdueTrend(ReportFilterDto f, int days)
        {
            var q = _repo.PurchaseInvoicesWithPartner();
            if (f.From.HasValue) q = q.Where(i => i.CreatedAt >= f.From.Value);
            if (f.To.HasValue) q = q.Where(i => i.CreatedAt <= f.To.Value);
            if (f.PartnerId.HasValue) q = q.Where(i => i.PartnerId == f.PartnerId.Value);

            var perDay = q.GroupBy(i => new { i.PartnerId, i.Partner.PartnerName, Day = i.CreatedAt!.Value.Date })
                .Select(g => new
                {
                    g.Key.PartnerId,
                    g.Key.PartnerName,
                    Day = g.Key.Day,
                    Overdue = g.Where(x => x.Status == "Overdue").Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Day)
                .ToList();

            var result = new List<OverdueTrendDto>();
            foreach (var grp in perDay.GroupBy(x => new { x.PartnerId, x.PartnerName }))
            {
                var s = grp.OrderBy(x => x.Day).ToList();
                if (s.Count < 3) continue;
                var xs = Enumerable.Range(0, s.Count).Select(i => (double)i).ToArray();
                var ys = s.Select(i => (double)i.Overdue).ToArray();
                double xAvg = xs.Average();
                double yAvg = ys.Average();
                double slope = xs.Zip(ys, (x, y) => (x - xAvg) * (y - yAvg)).Sum() / xs.Sum(x => Math.Pow(x - xAvg, 2));
                double intercept = yAvg - slope * xAvg;
                double nextX = s.Count + days;
                double predicted = Math.Max(0, slope * nextX + intercept);

                result.Add(new OverdueTrendDto
                {
                    PartnerId = grp.Key.PartnerId,
                    PartnerName = grp.Key.PartnerName ?? "",
                    CurrentOverdue = (decimal)ys.Last(),
                    ForecastOverdue = (decimal)predicted,
                    Trend = slope < 0 ? "Decreasing" : "Increasing"
                });
            }
            return result;
        }
    }
}
