namespace Domain.Models
{
    public class PriceMaterialPartner
    {
        public int PriceMaterialPartnerId { get; set; }
        public int PartnerId { get; set; }
        public int MaterialId { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
        public string Status { get; set; } = "Active";
        public Partner Partner { get; set; } = default!;
        public Material Material { get; set; } = default!;
    }
}
