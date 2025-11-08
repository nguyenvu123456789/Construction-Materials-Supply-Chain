namespace Domain.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }
    public string InvoiceCode { get; set; } = null!;
    public string InvoiceType { get; set; } = null!;
    public int PartnerId { get; set; }
    public int CreatedBy { get; set; }

    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ExportStatus { get; set; }
    public string? ImportStatus { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;
    public virtual Partner Partner { get; set; } = null!;
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
}
