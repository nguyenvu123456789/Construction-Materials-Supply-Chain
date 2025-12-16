namespace Application.DTOs
{
    public class CreateOrderDto
    {
        public int CreatedBy { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Note { get; set; }
        public string? PhoneNumber { get; set; }
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
        public List<CreateOrderMaterialDto> Materials { get; set; } = new();
    }

    public class CreateOrderMaterialDto
    {
        public int MaterialId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = null!;
        public string? SupplierName { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Note { get; set; }
        public List<OrderMaterialResponseDto> Materials { get; set; } = new();
    }

    public class OrderMaterialResponseDto
    {
        public int MaterialId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = null!;
    }

    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = "";
        public int Quantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public string? Status { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
    }


    public class OrderWithDetailsDto
    {
        public string OrderCode { get; set; } = null!;
        public int PartnerId { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? SupplierName { get; set; }
        public string? OrderStatus { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Note { get; set; }
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }

    public class HandleOrderRequestDto
    {
        public int OrderId { get; set; }
        public int HandledBy { get; set; }
        public string ActionType { get; set; } = null!; // "Approved" hoặc "Rejected"
        public string? Note { get; set; }
    }
}
