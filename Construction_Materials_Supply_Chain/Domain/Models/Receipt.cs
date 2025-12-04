namespace Domain.Models
{
    public class Receipt
    {
        public int ReceiptId { get; set; }
        public DateTime Date { get; set; }
        public int? PartnerId { get; set; }
        public int? InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Cash";
        public int? MoneyAccountId { get; set; }
        public string? Reference { get; set; }
        public string Status { get; set; } = "Draft";
    }
}
