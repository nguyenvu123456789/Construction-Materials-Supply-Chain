namespace Domain.Models
{
    public class PriceMaterialPartner
    {
        public int PriceMaterialPartnerId { get; set; }
        public int PartnerId { get; set; }
        public int MaterialId { get; set; }
        // Giá bán cơ 
        public decimal SellPrice { get; set; }
        // Giảm giá theo %
        public decimal DiscountPercent { get; set; } = 0;
        // Giảm giá theo tiền
        public decimal DiscountAmount { get; set; } = 0;
        public string Status { get; set; } = "Active";
        public Partner Partner { get; set; } = default!;
        public Material Material { get; set; } = default!;
    }
}
