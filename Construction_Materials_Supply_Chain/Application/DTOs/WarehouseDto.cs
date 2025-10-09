namespace Application.DTOs
{
    public class WarehouseDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public string? Location { get; set; }
        public int? ManagerId { get; set; }
    }

    public class WarehouseCreateDto
    {
        public string WarehouseName { get; set; } = null!;
        public string? Location { get; set; }
        public int? ManagerId { get; set; }
    }

    public class WarehouseUpdateDto
    {
        public string WarehouseName { get; set; } = null!;
        public string? Location { get; set; }
        public int? ManagerId { get; set; }
    }
}
