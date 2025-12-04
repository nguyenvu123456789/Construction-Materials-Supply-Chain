namespace Domain.Models
{
    public class Receipt
    {
        public int ReceiptId { get; set; }
        public string ReceiptNumber { get; set; } = "";
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? PostingDate { get; set; }
        public decimal Amount { get; set; }
        public string CustomerName { get; set; } = "";
        public string PaymentMethod { get; set; } = "Bank";
        public string? BankAccount { get; set; }
        public string Reference { get; set; } = "";
        public string Status { get; set; } = "Draft";
        public string CreatedBy { get; set; } = "";
        public int? PartnerId { get; set; }
        public string AmountInWords { get; set; }
    }
}
