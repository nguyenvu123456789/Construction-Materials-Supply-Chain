using BusinessObjects;

public partial class ImportRequest
{
    public int ImportRequestId { get; set; }
    public DateTime RequestDate { get; set; }
    public int WarehouseId { get; set; }
    public string Status { get; set; } = "Pending";
    public int RequestedBy { get; set; }
    public string? Notes { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual User RequestedByNavigation { get; set; } = null!;
    public virtual ICollection<ImportRequestDetail> Details { get; set; } = new List<ImportRequestDetail>();
}
