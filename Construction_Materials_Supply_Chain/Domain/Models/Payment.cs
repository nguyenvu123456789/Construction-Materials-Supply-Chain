namespace Domain.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public DateTime Date { get; set; }
        public int? PartnerId { get; set; }
        public int? InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Bank"; // "Cash"|"Bank"
        public int? MoneyAccountId { get; set; }
        public string? Reference { get; set; }
        public string Status { get; set; } = "Draft"; // Draft|Posted|Canceled
    }
}
