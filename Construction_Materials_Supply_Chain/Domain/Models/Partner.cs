using System.Text.Json.Serialization;

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
        public string? Region { get; set; }
        public virtual PartnerType PartnerType { get; set; } = null!;
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
        public virtual ICollection<Porter> Porters { get; set; } = new List<Porter>();
        public virtual ICollection<Transport> TransportsProvided { get; set; } = new List<Transport>();
        [JsonIgnore]
        public virtual ICollection<MaterialPartner> MaterialPartners { get; set; } = new List<MaterialPartner>();
    }
}
