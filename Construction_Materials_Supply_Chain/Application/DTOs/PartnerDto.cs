namespace Application.DTOs
{
    public class PartnerDto
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public string PartnerCode { get; set; } = null!;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public int PartnerTypeId { get; set; }
        public string PartnerTypeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class PartnerTypeDto
    {
        public int PartnerTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public List<PartnerDto> Partners { get; set; } = new();
    }

    public class PartnerUpdateDto
    {
        public string PartnerName { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public int PartnerTypeId { get; set; }
        public string? Status { get; set; }
    }

    public class PartnerCreateDto
    {
        public string PartnerName { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public int PartnerTypeId { get; set; }
        public string? Status { get; set; }
    }
}
