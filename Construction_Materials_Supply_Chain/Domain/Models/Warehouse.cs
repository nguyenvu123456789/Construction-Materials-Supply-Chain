namespace Domain.Models;

public partial class Warehouse
{
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public string? Location { get; set; }
    public int? ManagerId { get; set; }
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual ICollection<Import> Imports { get; set; } = new List<Import>();
    public virtual ICollection<Export> Exports { get; set; } = new List<Export>();
    public virtual User? Manager { get; set; }
    public virtual ICollection<MaterialCheck> MaterialChecks { get; set; }
        = new List<MaterialCheck>();
}
