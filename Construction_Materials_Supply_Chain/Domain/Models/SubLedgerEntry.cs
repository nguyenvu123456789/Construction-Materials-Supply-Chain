using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SubLedgerEntry
    {
        public int SubLedgerEntryId { get; set; }
        public string SubLedgerType { get; set; } = default!; // "AR"|"AP"
        public int PartnerId { get; set; }
        public int? InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public decimal Debit { get; set; }   // tăng công nợ
        public decimal Credit { get; set; }  // giảm công nợ
        public string? Reference { get; set; } // ví dụ "SalesInvoice#123"
    }
}
