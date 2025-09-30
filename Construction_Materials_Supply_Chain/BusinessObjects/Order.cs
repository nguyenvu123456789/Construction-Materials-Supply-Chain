namespace BusinessObjects;

public partial class Order
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string? CustomerName { get; set; }

    public int? SupplierId { get; set; }   // NEW
    public int? VendorId { get; set; }     // NEW

    public string? Status { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Supplier? Supplier { get; set; }   // NEW
    public virtual Vendor? Vendor { get; set; }       // NEW
    public virtual User? CreatedByNavigation { get; set; }
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<ShippingLog> ShippingLogs { get; set; } = new List<ShippingLog>();
}
