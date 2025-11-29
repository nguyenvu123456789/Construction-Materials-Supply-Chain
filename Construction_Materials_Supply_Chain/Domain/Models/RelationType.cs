namespace Domain.Models
{
    public class RelationType
    {
        public int RelationTypeId { get; set; }

        // Tên loại quan hệ: VIP, Preferred, Regular,...
        public string Name { get; set; } = null!;

        // Giảm giá theo %
        public decimal DiscountPercent { get; set; } = 0;

        // Giảm giá theo tiền
        public decimal DiscountAmount { get; set; } = 0;

        // Trạng thái
        public string Status { get; set; } = "Active";

        // Các quan hệ partner thuộc loại này
        public virtual ICollection<PartnerRelation> PartnerRelations { get; set; } = new List<PartnerRelation>();
    }
}
