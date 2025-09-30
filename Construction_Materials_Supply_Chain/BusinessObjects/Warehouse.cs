namespace BusinessObjects;

public partial class Warehouse
{
    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = null!;
    public string? Location { get; set; }

    public int? ManagerId { get; set; }
    public int? SupplierId { get; set; }
    public int? VendorId { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual User? Manager { get; set; }
    public virtual Supplier? Supplier { get; set; }
    public virtual Vendor? Vendor { get; set; }
}
