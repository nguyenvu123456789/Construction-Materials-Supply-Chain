namespace Domain.Models
{
    public class PartnerRelation
    {
        public int PartnerRelationId { get; set; }
        public int BuyerPartnerId { get; set; }
        public int SellerPartnerId { get; set; }
        public int RelationTypeId { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CooperationDate { get; set; }
        public Partner BuyerPartner { get; set; } = default!;
        public Partner SellerPartner { get; set; } = default!;
        public RelationType RelationType { get; set; } = default!;
    }
}
