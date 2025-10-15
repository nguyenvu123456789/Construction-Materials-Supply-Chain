namespace Domain.Models
{
    public class Receipt
    {
        public int ReceiptId { get; set; }
        public DateTime Date { get; set; }
        public int? PartnerId { get; set; }
        public int? InvoiceId { get; set; }   // tùy chọn: 1 phiếu ↔ 1 hóa đơn; nếu cần many-many, thêm Allocation sau
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Cash"; // "Cash"|"Bank"
        public int? MoneyAccountId { get; set; }
        public string? Reference { get; set; }
        public string Status { get; set; } = "Draft"; // Draft|Posted|Canceled
    }
}
