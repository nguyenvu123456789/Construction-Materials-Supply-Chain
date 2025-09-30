using BusinessObjects;

public partial class ExportRequestDetail
{
    public int ExportRequestDetailId { get; set; }
    public int ExportRequestId { get; set; }
    public int MaterialId { get; set; }
    public decimal Quantity { get; set; }

    public virtual ExportRequest ExportRequest { get; set; } = null!;
    public virtual Material Material { get; set; } = null!;
}
