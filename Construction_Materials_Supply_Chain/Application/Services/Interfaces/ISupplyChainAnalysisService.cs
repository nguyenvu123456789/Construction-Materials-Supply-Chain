using System;
using System.Collections.Generic;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface ISupplyChainAnalysisService
    {
        List<CategorySummaryDto> GetCategorySummary(DateTime from, DateTime to);
        List<SalesTrendPointDto> GetSalesTrend(SalesTrendFilterDto filter);
        List<LocationSummaryDto> GetLocationSummary(DateTime from, DateTime to);
        List<InventorySummaryDto> GetInventorySummary(DateTime from, DateTime to, int? partnerId = null);
        List<RecommendationDto> GetRecommendations(DateTime from, DateTime to, int? partnerId = null);
        List<StockForecastDto> GetDemandForecast(DateTime from, DateTime to, TimeGranularity granularity, int? materialId = null, int? partnerId = null);
    }
}
