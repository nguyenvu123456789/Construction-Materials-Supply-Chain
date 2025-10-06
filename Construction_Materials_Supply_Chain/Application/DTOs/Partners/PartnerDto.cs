namespace Application.DTOs.Partners
{
    public class PartnerDto
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        public int PartnerTypeId { get; set; }
        public string PartnerTypeName { get; set; } = string.Empty;
    }
}
