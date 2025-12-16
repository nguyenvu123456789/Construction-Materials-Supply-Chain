namespace Application.DTOs
{
    namespace Application.DTOs
    {
        public class PriceMaterialPartnerDto
        {
            public int PriceMaterialPartnerId { get; set; }
            public int PartnerId { get; set; }
            public string PartnerName { get; set; } = string.Empty;
            public int MaterialId { get; set; }
            public string MaterialName { get; set; } = string.Empty;
            public decimal SellPrice { get; set; }
            public decimal DiscountPercent { get; set; }
            public decimal DiscountAmount { get; set; }
            public string Status { get; set; } = "Active";
            public decimal PriceAfterDiscount { get; set; }
        }

        public class PriceMaterialPartnerUpdateDto
        {
            public int PriceMaterialPartnerId { get; set; }
            public decimal? SellPrice { get; set; }
            public decimal? DiscountPercent { get; set; }
            public decimal? DiscountAmount { get; set; }
            public string? Status { get; set; }
        }
        public class PriceCatalogQueryDto
        {
            public int? PartnerId { get; set; }
            public int? MaterialId { get; set; }
            public string? Status { get; set; }

            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;

            public string? SortBy { get; set; }
            public bool SortDesc { get; set; } = false;
            public decimal? MinPrice { get; set; }
            public decimal? MaxPrice { get; set; }
        }

        public class MaterialPriceDto
        {
            public int MaterialId { get; set; }
            public string MaterialCode { get; set; } = null!;
            public string MaterialName { get; set; } = null!;
            public decimal SellPrice { get; set; }
            public decimal DiscountPercent { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal PriceAfterDiscount { get; set; }
        }

    }
}
