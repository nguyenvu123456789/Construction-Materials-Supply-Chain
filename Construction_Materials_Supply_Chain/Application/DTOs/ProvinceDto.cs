namespace Application.DTOs
{
    public class ProvinceDto
    {
        public string Name { get; set; } = "";
        public List<WardDto>? Wards { get; set; }
    }
}
