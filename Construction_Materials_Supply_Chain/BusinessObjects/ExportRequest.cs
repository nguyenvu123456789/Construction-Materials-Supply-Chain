using BusinessObjects;

public partial class ExportRequest
{
    public int ExportRequestId { get; set; }
    public DateTime RequestDate { get; set; }
    public int WarehouseId { get; set; }
    public int RequestedBy { get; set; }
    public string? Notes { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual User RequestedByNavigation { get; set; } = null!;
    public virtual ICollection<ExportRequestDetail> Details { get; set; } = new List<ExportRequestDetail>();
}
