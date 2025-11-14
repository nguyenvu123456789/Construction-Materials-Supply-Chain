namespace Domain.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int MaterialId { get; set; }
    public int Quantity { get; set; }
    public string? Status { get; set; }
    public decimal? UnitPrice { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Material Material { get; set; } = null!;
}
