using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AlertRuleCreateDto
    {
        public int PartnerId { get; set; }
        public int? WarehouseId { get; set; }
        public int MaterialId { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal? CriticalMinQuantity { get; set; }
        public bool SendZalo { get; set; } = true;
        public int RecipientMode { get; set; } = 1;
        public int[] RoleIds { get; set; } = Array.Empty<int>();
        public int[] UserIds { get; set; } = Array.Empty<int>();
    }

    public class AlertRuleUpdateDto : AlertRuleCreateDto
    {
        public int RuleId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class RunAlertDto
    {
        public int PartnerId { get; set; }
    }
}
