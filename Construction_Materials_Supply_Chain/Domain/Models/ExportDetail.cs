namespace Domain.Models;

public partial class ExportDetail
{
    public int ExportDetailId { get; set; }
    public int ExportId { get; set; }

    // snapshot từ Material
    public string MaterialCode { get; set; } = null!;
    public string MaterialName { get; set; } = null!;
    public string? Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal LineTotal { get; set; }

    public virtual Export Export { get; set; } = null!;
}

