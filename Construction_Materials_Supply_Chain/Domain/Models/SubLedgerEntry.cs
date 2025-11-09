namespace Domain.Models
{
    public class SubLedgerEntry
    {
        public int SubLedgerEntryId { get; set; }
        public string SubLedgerType { get; set; } = default!;
        public int PartnerId { get; set; }
        public int? InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Reference { get; set; }
    }
}
