namespace Application.DTOs.Relations
{
    public class PartnerRelationDto
    {
        public int PartnerRelationId { get; set; }
        public int BuyerPartnerId { get; set; }
        public string BuyerPartnerName { get; set; } = string.Empty;

        public int SellerPartnerId { get; set; }
        public string SellerPartnerName { get; set; } = string.Empty;

        public int RelationTypeId { get; set; }
        public string RelationTypeName { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";
        public DateTime CooperationDate { get; set; }
    }

    public class PartnerRelationCreateDto
    {
        public int BuyerPartnerId { get; set; }
        public int SellerPartnerId { get; set; }
        public int RelationTypeId { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CooperationDate { get; set; } = DateTime.Now;
    }
    public class PartnerRelationUpdateDto
    {
        public int RelationTypeId { get; set; }
        public string Status { get; set; } = "Active";
    }
}
