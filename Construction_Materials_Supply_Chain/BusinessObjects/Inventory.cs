namespace BusinessObjects;

public partial class Inventory
{
    public int InventoryId { get; set; }
    public int WarehouseId { get; set; }
    public int MaterialId { get; set; }
    public int? Quantity { get; set; }
    public string? BatchNumber { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Location { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual Material Material { get; set; } = null!;
    public virtual Warehouse Warehouse { get; set; } = null!;
}