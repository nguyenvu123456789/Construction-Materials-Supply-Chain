using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Partners
{
    public class PartnerTypeDto
    {
        public int PartnerTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public List<PartnerDto> Partners { get; set; } = new();
    }
}
