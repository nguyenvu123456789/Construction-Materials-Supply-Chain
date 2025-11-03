using Application.Common.Pagination;

namespace Application.DTOs.Common.Pagination
{
    public class PriceCatalogQueryDto : PagedQueryDto
    {
        public int? PartnerId { get; set; }
        public int? MaterialId { get; set; }
        public int? CategoryId { get; set; }
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }
    }
}
