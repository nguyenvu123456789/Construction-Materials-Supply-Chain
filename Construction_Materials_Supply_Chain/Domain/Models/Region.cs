namespace Domain.Models
{
    public class Region
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; } = null!;
        public virtual ICollection<PartnerRegion> PartnerRegions { get; set; } = new List<PartnerRegion>();
    }
}
