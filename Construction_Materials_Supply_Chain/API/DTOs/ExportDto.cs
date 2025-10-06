namespace API.DTOs
{
    public class ExportDto
    {
        public DateTime ExportDate { get; set; }
        public int WarehouseId { get; set; }
        public int CreatedBy { get; set; }
        public string? Notes { get; set; }
    }
}
