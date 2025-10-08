namespace Application.DTOs
{
    public class ExportRequestDto
    {
        public int WarehouseId { get; set; }
        public int CreatedBy { get; set; }
        public string? Notes { get; set; }
        public List<ExportMaterialDto> Materials { get; set; } = new();
    }

    public class ExportMaterialDto
    {
        public int MaterialId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class ExportConfirmDto
    {
        public string ExportCode { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class ExportResponseDto
    {
        public int ExportId { get; set; }
        public string ExportCode { get; set; } = null!;
        public int WarehouseId { get; set; }
        public int CreatedBy { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime ExportDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ExportDetailResponseDto> Details { get; set; } = new();
    }

    public class ExportDetailResponseDto
    {
        public string MaterialCode { get; set; } = null!;
        public string MaterialName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
