namespace API.DTOs
{
    public class ImportRequestDto
    {
        public int ImportRequestId { get; set; }
        public DateTime RequestDate { get; set; }
        public int WarehouseId { get; set; }
        public string Status { get; set; } = "Pending";
        public int RequestedBy { get; set; }
        public string? Notes { get; set; }

        public ICollection<ImportRequestDetailDto> Details { get; set; } = new List<ImportRequestDetailDto>();
    }

    public class ImportRequestDetailDto
    {
        public int MaterialId { get; set; }
        public decimal Quantity { get; set; }
    }
}
