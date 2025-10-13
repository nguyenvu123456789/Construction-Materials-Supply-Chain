using Application.Common.Pagination;

namespace Application.DTOs.Common.Pagination
{
    public class StockCheckQueryDto : PagedQueryDto
    {
        public int? WarehouseId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
