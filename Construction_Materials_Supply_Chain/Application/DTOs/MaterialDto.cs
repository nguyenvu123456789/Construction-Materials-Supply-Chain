public class MaterialDto
{
    public int MaterialId { get; set; }
    public string? MaterialCode { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int PartnerId { get; set; }
    public string? Unit { get; set; }
    public DateTime? CreatedAt { get; set; }

    // Thông tin liên kết
    public string? CategoryName { get; set; }
    public string? PartnerName { get; set; }

    // 🆕 Thêm 2 trường này
    public int? Quantity { get; set; }
    public string? WarehouseName { get; set; }
}
