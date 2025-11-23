using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RegionDto
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; } = null!;
    }

    public class RegionCreateDto
    {
        public string RegionName { get; set; } = null!;
    }

    public class RegionUpdateDto
    {
        public string RegionName { get; set; } = null!;
    }
}
