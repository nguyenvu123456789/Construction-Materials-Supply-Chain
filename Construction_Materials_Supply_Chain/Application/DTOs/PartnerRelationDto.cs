
namespace Application.DTOs
{
    public class PartnerRelationDto
    {
        public int PartnerRelationId { get; set; }
        public int BuyerPartnerId { get; set; }
        public int SellerPartnerId { get; set; }
        public int RelationTypeId { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CooperationDate { get; set; }
    }
}
