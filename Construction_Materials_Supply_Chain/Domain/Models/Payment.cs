namespace Domain.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string PaymentNumber { get; set; } = "";
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? PostingDate { get; set; }
        public decimal Amount { get; set; }
        public string VendorName { get; set; } = "";
        public string PaymentMethod { get; set; } = "Bank";
        public string? BankAccount { get; set; }
        public string Reference { get; set; } = "";
        public string Status { get; set; } = "Draft";
        public string CreatedBy { get; set; } = "";
        public int? PartnerId { get; set; }
        public string AmountInWords { get; set; }
        public string PaymentType { get; set; } = "Vendor Payment";
    }
}
