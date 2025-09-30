using BusinessObjects;

public partial class ImportRequestDetail
{
    public int ImportRequestDetailId { get; set; }
    public int ImportRequestId { get; set; }
    public int MaterialId { get; set; }
    public decimal Quantity { get; set; }

    public virtual ImportRequest ImportRequest { get; set; } = null!;
    public virtual Material Material { get; set; } = null!;
}
