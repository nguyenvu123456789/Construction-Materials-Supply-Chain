using Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Common.Pagination
{
    public class StockCheckQueryDto : PagedQueryDto
    {
        public int? WarehouseId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
