namespace Domain.Models;

public partial class InvoiceDetail
{
    public int InvoiceDetailId { get; set; }

    public int InvoiceId { get; set; }

    public int MaterialId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? LineTotal { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Material Material { get; set; } = null!;
}
