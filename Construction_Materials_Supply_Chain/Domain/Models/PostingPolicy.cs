using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PostingPolicy
    {
        public int PostingPolicyId { get; set; }
        public string DocumentType { get; set; } = default!;  // "SalesInvoice","PurchaseInvoice","ExportCOGS","Receipt","Payment"
        public string RuleKey { get; set; } = default!;       // "Revenue","VATOut","AR","Inventory","VATIn","AP","COGS","Cash","Bank"
        public int DebitAccountId { get; set; }
        public int CreditAccountId { get; set; }
        public string? PartnerType { get; set; }
        public string? MaterialCategory { get; set; }
    }
}
