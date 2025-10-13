namespace Application.DTOs.Partners
{
    public class PartnerTypeDto
    {
        public int PartnerTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public List<PartnerDto> Partners { get; set; } = new();
    }
}
