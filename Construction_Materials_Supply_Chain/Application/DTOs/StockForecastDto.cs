namespace Application.DTOs
{
    public class StockForecastDto
    {
        public int MaterialId { get; set; }
        public int WarehouseId { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal ForecastQuantity { get; set; }
        public string Trend { get; set; } = string.Empty;
        public string ForecastStatus { get; set; } = string.Empty;
    }
}