using System;

namespace Application.DTOs
{
    public enum TimeGranularity
    {
        Day,
        Week,
        Month,
        Quarter,
        Year
    }

    public class CategorySummaryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal? TotalProfit { get; set; }
        public decimal? GrowthRatePercent { get; set; }
    }

    public class SalesTrendFilterDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public TimeGranularity Granularity { get; set; } = TimeGranularity.Day;
        public int? CategoryId { get; set; }
        public int? MaterialId { get; set; }
        public int? PartnerId { get; set; }
    }

    public class SalesTrendPointDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal? TotalProfit { get; set; }
    }

    public class LocationSummaryDto
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public string? Region { get; set; }
        public string? Status { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal? TotalProfit { get; set; }
        public decimal? GrowthRatePercent { get; set; }
    }

    public class InventorySummaryDto
    {
        public int MaterialId { get; set; }
        public string MaterialCode { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;

        public int OwnerPartnerId { get; set; }
        public string OwnerPartnerName { get; set; } = string.Empty;

        public decimal QuantityOnHand { get; set; }
        public decimal TotalSoldInPeriod { get; set; }
        public decimal RevenueInPeriod { get; set; }
        public decimal AverageInventory { get; set; }
        public decimal? TurnoverRate { get; set; }
        public bool IsFastMoving { get; set; }
        public bool IsSlowMoving { get; set; }
    }

    public enum RecommendationType
    {
        Replenish,
        ReduceStock,
        Promotion,
        FocusArea
    }

    public class RecommendationDto
    {
        public RecommendationType Type { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ExtraInfo { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
