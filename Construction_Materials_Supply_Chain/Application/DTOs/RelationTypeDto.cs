namespace Application.DTOs.RelationType
{
    public class RelationTypeDto
    {
        public int RelationTypeId { get; set; }
        public string Name { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Status { get; set; } = "Active";
    }
}
