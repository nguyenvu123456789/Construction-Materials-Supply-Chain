using Application.Common.Pagination;

namespace Application.DTOs
{
    // DTO dùng để trả về dữ liệu cho client
    public class MaterialDto
    {
        public int MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    // DTO dùng cho tạo hoặc cập nhật
    public class MaterialCreateUpdateDto
    {
        public string? MaterialCode { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int PartnerId { get; set; }
        public string? Unit { get; set; }
    }

    // DTO query (dùng để phân trang, tìm kiếm, lọc category)
    public class MaterialQueryDto : PagedQueryDto
    {
        public int? CategoryId { get; set; }
    }
}
