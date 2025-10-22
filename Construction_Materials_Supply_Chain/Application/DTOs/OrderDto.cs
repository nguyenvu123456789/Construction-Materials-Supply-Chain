namespace Application.DTOs
{
    public class CreateOrderDto
    {
        public int CreatedBy { get; set; }
        public string? DeliveryAddress { get; set; } 
        public string? Note { get; set; }
        public string? PhoneNumber { get; set; }
        public int SupplierId { get; set; }
        public List<OrderMaterialDto> Materials { get; set; } = new();
    }


    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = null!;
        public string? SupplierName { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Note { get; set; }
        public List<OrderMaterialDto> Materials { get; set; } = new();
    }

    public class OrderMaterialDto
    {
        public int MaterialId { get; set; }
        public int Quantity { get; set; }
    }
    public class OrderDetailDto
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
    }

    public class OrderWithDetailsDto
    {
        public string OrderCode { get; set; } = null!;
        public int PartnerId { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? SupplierName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Note { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
    }

    public class HandleOrderRequestDto
    {
        public int OrderId { get; set; }
        public int HandledBy { get; set; }
        public string ActionType { get; set; } = null!; // "Approved" hoặc "Rejected"
        public string? Note { get; set; }

        public int? TransportId { get; set; } // Chỉ dùng khi Approve
    }
}
