using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
