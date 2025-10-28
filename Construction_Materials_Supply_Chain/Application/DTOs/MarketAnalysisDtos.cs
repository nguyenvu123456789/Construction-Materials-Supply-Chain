namespace Application.DTOs
{
    public class MonthlyRevenueDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class TopMaterialDto
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = null!;
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class SupplierRevenueDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;
        public decimal TotalRevenue { get; set; }
    }

    public class StaffPerformanceDto
    {
        public int UserId { get; set; }
        public string Fullname { get; set; } = null!;
        public decimal TotalRevenue { get; set; }
    }

    public class RegionRevenueDto
    {
        public string Region { get; set; } = null!;
        public decimal TotalRevenue { get; set; }
    }
}
