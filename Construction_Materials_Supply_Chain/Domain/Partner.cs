namespace Domain;

public partial class Partner
{
    public int PartnerId { get; set; }
    public string PartnerName { get; set; } = null!;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public int PartnerTypeId { get; set; }

    public virtual PartnerType PartnerType { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
