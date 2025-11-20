using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProvinceDto
    {
        public string Name { get; set; } = "";
        public List<WardDto>? Wards { get; set; }
    }
}
