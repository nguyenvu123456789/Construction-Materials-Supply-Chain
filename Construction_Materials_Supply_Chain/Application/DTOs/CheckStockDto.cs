namespace Application.DTOs
{
    public class CheckStockRequestDto
    {
        public int WarehouseId { get; set; }
        public List<CheckStockItemDto> Items { get; set; } = new();
    }

    public class CheckStockItemDto
    {
        public int MaterialId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class CheckStockResultDto
    {
        public int MaterialId { get; set; }
        public decimal Requested { get; set; }
        public int Available { get; set; }
        public bool IsEnough { get; set; }
    }
}
