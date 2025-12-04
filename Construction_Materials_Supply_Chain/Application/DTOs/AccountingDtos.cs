namespace Application.DTOs
{
    public class ReceiptDTO
    {
        public string ReceiptNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime AccountingDate { get; set; }
        public string ReceiptType { get; set; }
        public int PartnerId { get; set; }
        public decimal Amount { get; set; }
        public string AmountInWords { get; set; }
        public string PaymentMethod { get; set; }
        public string BankAccount { get; set; }
        public string LinkedInvoiceIds { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
    }

    public class PaymentDTO
    {
        public string PaymentNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime AccountingDate { get; set; }
        public string PaymentType { get; set; }
        public int PartnerId { get; set; }
        public decimal Amount { get; set; }
        public string AmountInWords { get; set; }
        public string PaymentMethod { get; set; }
        public string BankAccountFrom { get; set; }
        public string BankAccountTo { get; set; }
        public string LinkedInvoiceIds { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public string RequestedBy { get; set; }
    }
}
