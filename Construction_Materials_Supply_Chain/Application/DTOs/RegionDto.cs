namespace Application.DTOs
{
    public class RegionDto
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; } = null!;
    }
    public class PartnerWithRegionsDto
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = null!;
        public List<RegionDto> Regions { get; set; } = new();
    }

    public class RegionCreateDto
    {
        public string RegionName { get; set; } = null!;
    }

    public class RegionUpdateDto
    {
        public string RegionName { get; set; } = null!;
    }
}
