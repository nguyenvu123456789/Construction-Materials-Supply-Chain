using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Warehouse
{
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public string? Location { get; set; }
    public int? ManagerId { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual ICollection<ImportRequest> ImportRequests { get; set; } = new List<ImportRequest>();
    public virtual ICollection<ExportRequest> ExportRequests { get; set; } = new List<ExportRequest>();

    public virtual User? Manager { get; set; }
}
