using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Application.Services.Implements
{
    public class SupplyChainAnalysisService : ISupplyChainAnalysisService
    {
        private readonly IInvoiceRepository _invoices;
        private readonly IMaterialRepository _materials;
        private readonly IInventoryRepository _inventories;
        private readonly IImportDetailRepository _importDetails;
        private readonly IWarehouseRepository _warehouses;
        private readonly IMapper _mapper;

        public SupplyChainAnalysisService(
            IInvoiceRepository invoices,
            IMaterialRepository materials,
            IInventoryRepository inventories,
            IImportDetailRepository importDetails,
            IWarehouseRepository warehouses,
            IMapper mapper)
        {
            _invoices = invoices;
            _materials = materials;
            _inventories = inventories;
            _importDetails = importDetails;
            _warehouses = warehouses;
            _mapper = mapper;
        }

        public List<CategorySummaryDto> GetCategorySummary(DateTime from, DateTime to)
        {
            var materials = _materials.GetAllWithInclude();
            var materialDict = materials.ToDictionary(m => m.MaterialId);

            var importDetails = _importDetails.GetAll();
            var costByMaterial = importDetails
                .GroupBy(d => d.MaterialId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var totalQty = g.Sum(x => x.Quantity);
                        var totalCost = g.Sum(x => x.LineTotal);
                        if (totalQty == 0) return 0m;
                        return totalCost / totalQty;
                    });

            var currentInvoices = _invoices.GetAllWithDetails()
                .Where(i => i.ExportStatus == "Success" && i.InvoiceType == "Export" && i.IssueDate >= from && i.IssueDate <= to)
                .ToList();

            var currentData = currentInvoices
                .SelectMany(i => i.InvoiceDetails, (i, d) =>
                {
                    if (!materialDict.TryGetValue(d.MaterialId, out var material)) return null;
                    var categoryId = material.CategoryId;
                    var categoryName = material.Category.CategoryName;
                    var quantity = Convert.ToDecimal(d.Quantity);
                    var revenue = d.LineTotal ?? quantity * d.UnitPrice;
                    decimal unitCost = 0m;
                    if (costByMaterial.TryGetValue(d.MaterialId, out var avgCost)) unitCost = avgCost;
                    var profit = revenue - unitCost * quantity;
                    return new
                    {
                        categoryId,
                        categoryName,
                        quantity,
                        revenue,
                        profit
                    };
                })
                .Where(x => x != null)
                .GroupBy(x => new { x!.categoryId, x.categoryName })
                .Select(g => new CategorySummaryDto
                {
                    CategoryId = g.Key.categoryId,
                    CategoryName = g.Key.categoryName,
                    TotalQuantity = g.Sum(x => x!.quantity),
                    TotalRevenue = g.Sum(x => x!.revenue),
                    TotalProfit = g.Sum(x => x!.profit),
                    GrowthRatePercent = 0m
                })
                .ToList();

            var previousRange = GetPreviousPeriod(from, to);
            var previousInvoices = _invoices.GetAllWithDetails()
                .Where(i => i.ExportStatus == "Success" && i.InvoiceType == "Export" && i.IssueDate >= previousRange.from && i.IssueDate <= previousRange.to)
                .ToList();

            var previousRevenueByCategory = previousInvoices
                .SelectMany(i => i.InvoiceDetails, (i, d) =>
                {
                    if (!materialDict.TryGetValue(d.MaterialId, out var material)) return null;
                    var categoryId = material.CategoryId;
                    var revenue = d.LineTotal ?? Convert.ToDecimal(d.Quantity) * d.UnitPrice;
                    return new
                    {
                        categoryId,
                        revenue
                    };
                })
                .Where(x => x != null)
                .GroupBy(x => x!.categoryId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x!.revenue));

            foreach (var item in currentData)
            {
                decimal previous = 0m;
                if (previousRevenueByCategory.TryGetValue(item.CategoryId, out var prev))
                    previous = prev;

                item.GrowthRatePercent = CalculateGrowthPercent(item.TotalRevenue, previous);
            }

            return currentData
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();
        }

        public List<SalesTrendPointDto> GetSalesTrend(SalesTrendFilterDto filter)
        {
            var materials = _materials.GetAllWithInclude();
            var materialDict = materials.ToDictionary(m => m.MaterialId);

            var invoices = _invoices.GetAllWithDetails()
                .Where(i => i.ExportStatus == "Success" && i.InvoiceType == "Export" && i.IssueDate >= filter.From && i.IssueDate <= filter.To)
                .ToList();

            var data = invoices
                .SelectMany(i => i.InvoiceDetails, (i, d) =>
                {
                    if (filter.MaterialId.HasValue && d.MaterialId != filter.MaterialId.Value) return null;
                    if (filter.CategoryId.HasValue)
                    {
                        if (!materialDict.TryGetValue(d.MaterialId, out var material)) return null;
                        if (material.CategoryId != filter.CategoryId.Value) return null;
                    }
                    if (filter.PartnerId.HasValue && i.PartnerId != filter.PartnerId.Value) return null;
                    var quantity = Convert.ToDecimal(d.Quantity);
                    var revenue = d.LineTotal ?? quantity * d.UnitPrice;
                    return new
                    {
                        i.IssueDate,
                        quantity,
                        revenue
                    };
                })
                .Where(x => x != null)
                .Select(x =>
                {
                    var range = GetPeriodRange(x!.IssueDate, filter.Granularity);
                    return new
                    {
                        range.Start,
                        range.End,
                        x.quantity,
                        x.revenue
                    };
                })
                .GroupBy(x => new { x.Start, x.End })
                .Select(g => new SalesTrendPointDto
                {
                    PeriodStart = g.Key.Start,
                    PeriodEnd = g.Key.End,
                    TotalQuantity = g.Sum(x => x.quantity),
                    TotalRevenue = g.Sum(x => x.revenue),
                    TotalProfit = 0m
                })
                .OrderBy(x => x.PeriodStart)
                .ToList();

            return data;
        }

        public List<LocationSummaryDto> GetLocationSummary(DateTime from, DateTime to)
        {
            var invoicesCurrent = _invoices.GetAllWithDetails()
                .Where(i => i.ExportStatus == "Success" && i.InvoiceType == "Export" && i.IssueDate >= from && i.IssueDate <= to)
                .ToList();

            var groupedCurrent = invoicesCurrent
                .Where(i => i.Partner != null)
                .SelectMany(i => i.InvoiceDetails, (i, d) => new
                {
                    i.PartnerId,
                    Partner = i.Partner,
                    Quantity = Convert.ToDecimal(d.Quantity),
                    Revenue = d.LineTotal ?? Convert.ToDecimal(d.Quantity) * d.UnitPrice
                })
                .GroupBy(x => x.PartnerId);

            var currentData = new List<LocationSummaryDto>();

            foreach (var g in groupedCurrent)
            {
                var partner = g.First().Partner;
                var dto = _mapper.Map<LocationSummaryDto>(partner);
                dto.PartnerId = partner.PartnerId;
                dto.TotalQuantity = g.Sum(x => x.Quantity);
                dto.TotalRevenue = g.Sum(x => x.Revenue);
                dto.TotalProfit = 0m;
                dto.GrowthRatePercent = 0m;
                currentData.Add(dto);
            }

            var previousRange = GetPreviousPeriod(from, to);
            var invoicesPrevious = _invoices.GetAllWithDetails()
                .Where(i => i.ExportStatus == "Success" && i.InvoiceType == "Export" && i.IssueDate >= previousRange.from && i.IssueDate <= previousRange.to)
                .ToList();

            var previousRevenueByPartner = invoicesPrevious
                .SelectMany(i => i.InvoiceDetails, (i, d) => new
                {
                    i.PartnerId,
                    Revenue = d.LineTotal ?? Convert.ToDecimal(d.Quantity) * d.UnitPrice
                })
                .GroupBy(x => x.PartnerId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.Revenue));

            foreach (var item in currentData)
            {
                decimal previous = 0m;
                if (previousRevenueByPartner.TryGetValue(item.PartnerId, out var prev))
                    previous = prev;

                item.GrowthRatePercent = CalculateGrowthPercent(item.TotalRevenue, previous);
            }

            return currentData
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();
        }

        public List<InventorySummaryDto> GetInventorySummary(DateTime from, DateTime to, int? partnerId = null)
        {
            var materials = _materials.GetAllWithInclude();
            var materialDict = materials.ToDictionary(m => m.MaterialId, m => m);

            var warehouses = _warehouses.GetAll();
            var warehouseDict = warehouses.ToDictionary(w => w.WarehouseId, w => w);

            List<Inventory> inventories;
            if (partnerId.HasValue)
            {
                inventories = _inventories.GetAllByPartnerId(partnerId.Value);
            }
            else
            {
                inventories = _inventories.GetAll();
            }

            var invoices = _invoices.GetAllWithDetails()
                .Where(i => i.ExportStatus == "Success" && i.InvoiceType == "Export" && i.IssueDate >= from && i.IssueDate <= to)
                .ToList();

            var soldByMaterial = invoices
                .SelectMany(i => i.InvoiceDetails, (i, d) => new
                {
                    d.MaterialId,
                    Quantity = Convert.ToDecimal(d.Quantity),
                    Revenue = d.LineTotal ?? Convert.ToDecimal(d.Quantity) * d.UnitPrice
                })
                .GroupBy(x => x.MaterialId)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Quantity = g.Sum(x => x.Quantity),
                        Revenue = g.Sum(x => x.Revenue)
                    });

            var result = new List<InventorySummaryDto>();

            foreach (var inv in inventories)
            {
                if (!materialDict.TryGetValue(inv.MaterialId, out var material)) continue;
                if (!warehouseDict.TryGetValue(inv.WarehouseId, out var warehouse)) continue;

                var dto = _mapper.Map<InventorySummaryDto>(inv);

                dto.MaterialCode = material.MaterialCode;
                dto.MaterialName = material.MaterialName;
                dto.CategoryName = material.Category.CategoryName;
                dto.WarehouseName = warehouse.WarehouseName;

                var ownerPartnerId = 0;
                var ownerPartnerName = string.Empty;
                var manager = warehouse.Manager;
                if (manager != null)
                {
                    if (manager.PartnerId.HasValue)
                        ownerPartnerId = manager.PartnerId.Value;
                    if (manager.Partner != null)
                        ownerPartnerName = manager.Partner.PartnerName;
                }
                dto.OwnerPartnerId = ownerPartnerId;
                dto.OwnerPartnerName = ownerPartnerName;

                decimal soldQty = 0m;
                decimal soldRevenue = 0m;
                if (soldByMaterial.TryGetValue(inv.MaterialId, out var sold))
                {
                    soldQty = sold.Quantity;
                    soldRevenue = sold.Revenue;
                }

                dto.TotalSoldInPeriod = soldQty;
                dto.RevenueInPeriod = soldRevenue;

                dto.AverageInventory = dto.QuantityOnHand;

                if (dto.AverageInventory > 0m)
                {
                    dto.TurnoverRate = soldQty / dto.AverageInventory;
                }
                else
                {
                    dto.TurnoverRate = 0m;
                }

                dto.IsFastMoving = dto.TurnoverRate >= 4m;
                dto.IsSlowMoving = dto.TurnoverRate <= 1m;

                result.Add(dto);
            }

            return result
                .OrderByDescending(x => x.TurnoverRate)
                .ToList();
        }

        public List<RecommendationDto> GetRecommendations(DateTime from, DateTime to, int? partnerId = null)
        {
            var inventorySummary = GetInventorySummary(from, to, partnerId);
            var recommendations = new List<RecommendationDto>();

            foreach (var item in inventorySummary)
            {
                if (item.QuantityOnHand < item.TotalSoldInPeriod && item.IsFastMoving)
                {
                    recommendations.Add(new RecommendationDto
                    {
                        Type = RecommendationType.Replenish,
                        Code = item.MaterialCode,
                        Name = item.MaterialName,
                        ExtraInfo = $"OnHand={item.QuantityOnHand};Sold={item.TotalSoldInPeriod}",
                        Reason = "Hàng bán nhanh, tồn kho thấp so với nhu cầu"
                    });
                }

                if (item.QuantityOnHand > item.TotalSoldInPeriod * 2 && item.IsSlowMoving)
                {
                    recommendations.Add(new RecommendationDto
                    {
                        Type = RecommendationType.ReduceStock,
                        Code = item.MaterialCode,
                        Name = item.MaterialName,
                        ExtraInfo = $"OnHand={item.QuantityOnHand};Sold={item.TotalSoldInPeriod}",
                        Reason = "Hàng tồn kho cao và bán chậm"
                    });
                }

                if (item.RevenueInPeriod > 0 && item.IsSlowMoving)
                {
                    recommendations.Add(new RecommendationDto
                    {
                        Type = RecommendationType.Promotion,
                        Code = item.MaterialCode,
                        Name = item.MaterialName,
                        ExtraInfo = $"Revenue={item.RevenueInPeriod}",
                        Reason = "Hàng có doanh thu nhưng tốc độ bán chậm, nên đẩy khuyến mãi"
                    });
                }
            }

            var locationSummary = GetLocationSummary(from, to)
                .OrderByDescending(x => x.TotalRevenue)
                .Take(5)
                .ToList();

            foreach (var loc in locationSummary)
            {
                recommendations.Add(new RecommendationDto
                {
                    Type = RecommendationType.FocusArea,
                    Code = loc.PartnerId.ToString(),
                    Name = loc.PartnerName,
                    ExtraInfo = $"Region={loc.Region};Revenue={loc.TotalRevenue}",
                    Reason = "Khu vực hoặc đối tác có doanh thu cao, nên tập trung nguồn lực bán hàng"
                });
            }

            return recommendations;
        }

        public List<StockForecastDto> GetDemandForecast(DateTime from, DateTime to, TimeGranularity granularity, int? materialId = null, int? partnerId = null)
        {
            var invoices = _invoices.GetAllWithDetails()
                .Where(i => i.ExportStatus == "Success" && i.InvoiceType == "Export" && i.IssueDate >= from && i.IssueDate <= to)
                .ToList();

            if (partnerId.HasValue)
            {
                invoices = invoices.Where(i => i.PartnerId == partnerId.Value).ToList();
            }

            var series = invoices
                .SelectMany(i => i.InvoiceDetails, (i, d) =>
                {
                    if (materialId.HasValue && d.MaterialId != materialId.Value) return null;
                    var quantity = Convert.ToDecimal(d.Quantity);
                    var range = GetPeriodRange(i.IssueDate, granularity);
                    return new
                    {
                        d.MaterialId,
                        range.Start,
                        range.End,
                        quantity
                    };
                })
                .Where(x => x != null)
                .GroupBy(x => new { x!.MaterialId, x.Start, x.End })
                .Select(g => new
                {
                    g.Key.MaterialId,
                    g.Key.Start,
                    g.Key.End,
                    Quantity = g.Sum(x => x.quantity)
                })
                .GroupBy(x => x.MaterialId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.Start).ToList());

            var inventories = partnerId.HasValue
                ? _inventories.GetAllByPartnerId(partnerId.Value)
                : _inventories.GetAll();

            var result = new List<StockForecastDto>();

            foreach (var inv in inventories)
            {
                if (!series.TryGetValue(inv.MaterialId, out var points)) continue;
                if (points.Count == 0) continue;

                decimal currentQty = inv.Quantity ?? 0m;
                decimal lastQty = points.Last().Quantity;
                decimal forecastQty;
                string trend;
                
                if (points.Count >= 2)
                {
                    var prevQty = points[points.Count - 2].Quantity;
                    var slope = lastQty - prevQty;
                    forecastQty = lastQty + slope;
                    if (slope > 0) trend = "Tăng";
                    else if (slope < 0) trend = "Giảm";
                    else trend = "Ổn định";
                }
                else
                {
                    forecastQty = lastQty;
                    trend = "Ổn định";
                }

                if (forecastQty < 0) forecastQty = 0;

                string status;
                if (forecastQty > currentQty)
                {
                    status = "Nguy cơ thiếu hàng";
                }
                else if (forecastQty < currentQty / 2)
                {
                    status = "Dư hàng";
                }
                else
                {
                    status = "Bình thường";
                }

                var dto = new StockForecastDto
                {
                    MaterialId = inv.MaterialId,
                    WarehouseId = inv.WarehouseId,
                    CurrentQuantity = currentQty,
                    ForecastQuantity = forecastQty,
                    Trend = trend,
                    ForecastStatus = status
                };

                result.Add(dto);
            }

            return result;
        }

        private (DateTime from, DateTime to) GetPreviousPeriod(DateTime from, DateTime to)
        {
            var lengthTicks = (to - from).Ticks;
            if (lengthTicks <= 0)
            {
                return (from, from);
            }

            var minTicks = DateTime.MinValue.Ticks;
            var prevFromTicks = from.Ticks - lengthTicks;

            if (prevFromTicks < minTicks)
            {
                return (DateTime.MinValue, from);
            }

            var previousFrom = new DateTime(prevFromTicks, from.Kind);
            var previousTo = from;
            return (previousFrom, previousTo);
        }

        private (DateTime Start, DateTime End) GetPeriodRange(DateTime date, TimeGranularity granularity)
        {
            if (granularity == TimeGranularity.Day)
            {
                var start = date.Date;
                var end = start.AddDays(1).AddTicks(-1);
                return (start, end);
            }

            if (granularity == TimeGranularity.Week)
            {
                var culture = CultureInfo.CurrentCulture;
                var calendar = culture.Calendar;
                var weekNum = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                var year = date.Year;
                var weekStart = FirstDateOfWeek(year, weekNum);
                var weekEnd = weekStart.AddDays(6);
                return (weekStart, weekEnd);
            }

            if (granularity == TimeGranularity.Month)
            {
                var start = new DateTime(date.Year, date.Month, 1);
                var end = start.AddMonths(1).AddDays(-1);
                return (start, end);
            }

            if (granularity == TimeGranularity.Quarter)
            {
                var quarter = (date.Month - 1) / 3 + 1;
                var firstMonth = (quarter - 1) * 3 + 1;
                var start = new DateTime(date.Year, firstMonth, 1);
                var end = start.AddMonths(3).AddDays(-1);
                return (start, end);
            }

            var yearStart = new DateTime(date.Year, 1, 1);
            var yearEnd = new DateTime(date.Year, 12, 31);
            return (yearStart, yearEnd);
        }

        private DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            var firstMonday = jan1.AddDays(daysOffset);
            var firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            if (firstWeek <= 1)
            {
                weekOfYear -= 1;
            }
            return firstMonday.AddDays(weekOfYear * 7);
        }

        private decimal CalculateGrowthPercent(decimal currentRevenue, decimal previousRevenue)
        {
            if (currentRevenue <= 0m && previousRevenue <= 0m)
                return 0m;

            if (previousRevenue <= 0m && currentRevenue > 0m)
                return 100m;

            return (currentRevenue - previousRevenue) * 100m / previousRevenue;
        }
    }
}
