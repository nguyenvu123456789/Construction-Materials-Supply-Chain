namespace Domain.Models
{
    public class PartnerRegion
    {
        public int PartnerRegionId { get; set; }
        public int PartnerId { get; set; }
        public int RegionId { get; set; }
        public virtual Partner Partner { get; set; } = null!;
        public virtual Region Region { get; set; } = null!;
    }
}
