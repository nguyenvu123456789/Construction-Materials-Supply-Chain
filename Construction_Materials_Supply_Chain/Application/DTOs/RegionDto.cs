namespace Application.DTOs
{
    public class RegionDto
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; } = null!;
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
