namespace Domain.Models
{
    public partial class Partner
    {
        public int PartnerId { get; set; }
        public string PartnerCode { get; set; } = null!;
        public string PartnerName { get; set; } = null!;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public int PartnerTypeId { get; set; }
        public string? Status { get; set; } = "Active";
        public virtual PartnerType PartnerType { get; set; } = null!;

        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
