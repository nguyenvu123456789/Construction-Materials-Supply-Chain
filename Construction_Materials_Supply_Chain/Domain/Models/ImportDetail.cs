namespace Domain.Models;

public partial class ImportDetail
{
    public int ImportDetailId { get; set; }
    public int ImportId { get; set; }
    public int MaterialId { get; set; }

    // snapshot từ Material
    public string MaterialCode { get; set; } = null!;
    public string MaterialName { get; set; } = null!;
    public string? Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal LineTotal { get; set; }

    public virtual Import Import { get; set; } = null!;
    public virtual Material Material { get; set; } = null!;
}
