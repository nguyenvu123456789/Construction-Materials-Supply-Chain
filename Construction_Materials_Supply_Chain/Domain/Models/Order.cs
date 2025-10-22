namespace Domain.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string OrderCode { get; set; } = null!;

    public string? CustomerName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? Status { get; set; }
    public string? Note { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? SupplierId { get; set; } 
    public virtual Partner? Supplier { get; set; } 
    public virtual User? CreatedByNavigation { get; set; }
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<ShippingLog> ShippingLogs { get; set; } = new List<ShippingLog>();
}
