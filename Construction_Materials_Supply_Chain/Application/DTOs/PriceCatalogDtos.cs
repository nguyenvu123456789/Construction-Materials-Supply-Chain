namespace Application.DTOs
{
    namespace Application.DTOs
    {
        public class PriceMaterialPartnerDto
        {
            public int PartnerId { get; set; }
            public string PartnerName { get; set; } = default!;
            public int MaterialId { get; set; }
            public string MaterialCode { get; set; } = default!;
            public string MaterialName { get; set; } = default!;
            public string CategoryName { get; set; } = "";
            public decimal? BuyPrice { get; set; }
            public decimal? SellPrice { get; set; }
            public string Status { get; set; } = "Active";
        }

        public class PriceMaterialPartnerUpdateDto
        {
            public int PartnerId { get; set; }
            public int MaterialId { get; set; }
            public decimal BuyPrice { get; set; }
            public decimal SellPrice { get; set; }
            public string? Status { get; set; }
        }
    }
}
