namespace Application.DTOs
{
    public class ImportRequestDto
    {
        public string? ImportCode { get; set; } 
        public string? InvoiceCode { get; set; }
        public int WarehouseId { get; set; }
        public int CreatedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class ImportResponseDto
    {
        public int ImportId { get; set; }
        public string ImportCode { get; set; } = null!;
        public string? InvoiceCode { get; set; }
        public int WarehouseId { get; set; }
        public int CreatedBy { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime ImportDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class PendingImportMaterialDto
    {
        public int MaterialId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CreatePendingImportDto
    {
        public int WarehouseId { get; set; }
        public int CreatedBy { get; set; }
        public string? Notes { get; set; }

        // Danh sách vật tư yêu cầu nhập
        public List<PendingImportMaterialDto> Materials { get; set; } = new();
    }

    public class PendingImportResponseDto
    {
        public int ImportId { get; set; }
        public string ImportCode { get; set; } = null!;
        public int WarehouseId { get; set; }
        public int CreatedBy { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<PendingImportMaterialResponseDto> Materials { get; set; } = new();
    }

    public class PendingImportMaterialResponseDto
    {
        public string MaterialCode { get; set; } = null!;
        public string MaterialName { get; set; } = null!;
        public string? Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

}
