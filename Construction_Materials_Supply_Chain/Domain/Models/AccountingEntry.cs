namespace Domain.Models
{
    public class AccountingEntry
    {
        public int Id { get; set; }
        public DateTime EntryDate { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Description { get; set; }

        public int? PartnerId { get; set; }
        public string PartnerName { get; set; }
    }
}
