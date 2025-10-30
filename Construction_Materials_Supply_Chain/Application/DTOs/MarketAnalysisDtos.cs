namespace Application.DTOs
{
    namespace Application.DTOs
    {
        public class WeeklyRevenueDto
        {
            public int PartnerId { get; set; }
            public string PartnerName { get; set; } = string.Empty;
            public DateTime WeekStart { get; set; }
            public DateTime WeekEnd { get; set; }
            public decimal TotalRevenue { get; set; }
        }
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
}
