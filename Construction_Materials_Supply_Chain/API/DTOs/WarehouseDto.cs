namespace API.DTOs
{
    public class WarehouseDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string Location { get; set; }
        public int? ManagerId { get; set; }
    }
}
