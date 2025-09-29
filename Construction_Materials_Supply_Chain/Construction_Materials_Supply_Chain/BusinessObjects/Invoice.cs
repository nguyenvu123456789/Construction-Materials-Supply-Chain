using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public string InvoiceType { get; set; } = null!;

    public int? RelatedOrderId { get; set; }

    public int? CustomerId { get; set; }

    public int? SupplierId { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime? DueDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? Customer { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual Order? RelatedOrder { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
