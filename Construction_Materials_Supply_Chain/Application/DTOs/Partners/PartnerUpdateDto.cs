namespace Application.DTOs.Partners
{
    public class PartnerUpdateDto
    {
        public string PartnerName { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public int PartnerTypeId { get; set; }
    }
}
