namespace Application.DTOs
{
    public class InventoryInfoDto
    {
        public int MaterialId { get; set; }
        public string MaterialCode { get; set; } = null!;
        public string MaterialName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string? BatchNumber { get; set; }
        public DateOnly? ExpiryDate { get; set; }
    }
}
