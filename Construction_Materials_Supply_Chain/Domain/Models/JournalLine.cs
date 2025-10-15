using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class JournalLine
    {
        public int JournalLineId { get; set; }
        public int JournalEntryId { get; set; }
        public JournalEntry JournalEntry { get; set; } = default!;
        public int AccountId { get; set; }
        public Account Account { get; set; } = default!;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public int? PartnerId { get; set; }   // soft link → Partner
        public int? InvoiceId { get; set; }   // soft link → Invoice
        public string? LineMemo { get; set; }
    }
}
