namespace API.DTOs
{
    public class ExportRequestDto
    {
        public int ExportRequestId { get; set; }
        public DateTime RequestDate { get; set; }
        public int WarehouseId { get; set; }
        public string Status { get; set; } = "Pending";
        public int RequestedBy { get; set; }
        public string? Notes { get; set; }

        public ICollection<ExportRequestDetailDto> Details { get; set; } = new List<ExportRequestDetailDto>();
    }

    public class ExportRequestDetailDto
    {
        public int MaterialId { get; set; }
        public decimal Quantity { get; set; }
    }
}
