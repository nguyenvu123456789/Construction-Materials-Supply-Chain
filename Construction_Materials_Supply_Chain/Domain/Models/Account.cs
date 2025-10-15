using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!; // Asset|Liability|Equity|Revenue|Expense
        public bool IsPosting { get; set; } = true;
        public int? ParentId { get; set; }
        public Account? Parent { get; set; }
        public ICollection<Account> Children { get; set; } = new List<Account>();
    }
}
