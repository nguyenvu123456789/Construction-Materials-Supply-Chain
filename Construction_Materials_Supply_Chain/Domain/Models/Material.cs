namespace Domain.Models;

public partial class Material
{
    public int MaterialId { get; set; }
    public string? MaterialCode { get; set; }
    public string MaterialName { get; set; } = null!;
    public int CategoryId { get; set; }
    public int PartnerId { get; set; }
    public string? Unit { get; set; }
    public DateTime? CreatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;
    public virtual Partner Partner { get; set; } = null!;

    public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();
    public virtual ICollection<ImportReportDetail> ImportReportDetails { get; set; } = new List<ImportReportDetail>();
    public virtual ICollection<ExportDetail> ExportDetails { get; set; } = new List<ExportDetail>();
    public virtual ICollection<ExportReportDetail> ExportReportDetails { get; set; } = new List<ExportReportDetail>();
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<MaterialCheck> MaterialChecks { get; set; } = new List<MaterialCheck>();
}
