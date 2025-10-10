namespace Domain.Models
{
    public partial class PartnerType
    {
        public int PartnerTypeId { get; set; }
        public string TypeName { get; set; } = null!;

        public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();
    }
}
