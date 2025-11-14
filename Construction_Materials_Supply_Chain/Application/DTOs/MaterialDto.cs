
namespace Application.DTOs.Material
{
    public class CreateMaterialRequest
    {
        public string MaterialCode { get; set; } = null!;
        public string MaterialName { get; set; } = null!;
        public int CategoryId { get; set; }
        public int PartnerId { get; set; }
        public string Unit { get; set; } = null!;
        public int WarehouseId { get; set; }
    }
    public class MaterialDto
    {
        public int MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int PartnerId { get; set; }
        public string? Unit { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CategoryName { get; set; }
        public string? PartnerName { get; set; }
        public int? Quantity { get; set; }
        public string? WarehouseName { get; set; }
    }
    public class UpdateMaterialRequest
    {
        public string MaterialCode { get; set; } = null!;
        public string MaterialName { get; set; } = null!;
        public int CategoryId { get; set; }
        public int PartnerId { get; set; }
        public string Unit { get; set; } = null!;
        public string Status { get; set; } = "Active";
    }
    public class MaterialDetailResponse
    {
        public MaterialDto Material { get; set; } = null!;
        public List<PartnerDto> Partners { get; set; } = new();
    }
}
