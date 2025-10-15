using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class JournalEntry
    {
        public int JournalEntryId { get; set; }
        public DateTime PostingDate { get; set; }
        public string SourceType { get; set; } = default!; // "SalesInvoice","PurchaseInvoice","Receipt","Payment","ExportCOGS"...
        public int SourceId { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Memo { get; set; }
        public ICollection<JournalLine> Lines { get; set; } = new List<JournalLine>();
    }
}
