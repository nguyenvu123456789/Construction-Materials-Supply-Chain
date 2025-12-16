namespace Application.DTOs
{
    public class ReceiptDTO
    {
        public DateTime DateCreated { get; set; }
        public DateTime AccountingDate { get; set; }
        public string ReceiptType { get; set; }
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string Address { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string BankAccount { get; set; }
        public string Invoices { get; set; }
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string Payee { get; set; }
        public string Notes { get; set; }
    }

    public class PaymentDTO
    {
        public DateTime DateCreated { get; set; }
        public DateTime AccountingDate { get; set; }
        public string PaymentType { get; set; }
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string TaxCode { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string BankAccountFrom { get; set; }
        public string BankAccountTo { get; set; }
        public string Invoices { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public string RequestedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string PaidBy { get; set; }
        public string Recipient { get; set; }
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
        public string CreatedBy { get; set; }
        public string Notes { get; set; }
        public string Account { get; set; }
    }

    public class LedgerEntryDto
    {
        public int No { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime PostingDate { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string ContraAccount { get; set; }
        public string PartnerName { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string BalanceType { get; set; }
    }

    public class APAgingDto
    {
        public string InvoiceCode { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int OverdueDays { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; }
    }

    public class CashBookDto
    {
        public int No { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime PostingDate { get; set; }
        public string Type { get; set; }
        public string DocumentNumber { get; set; }
        public string Partner { get; set; }
        public string ContraAccount { get; set; }
        public string Description { get; set; }
        public decimal InAmount { get; set; }
        public decimal OutAmount { get; set; }
        public decimal Balance { get; set; }
        public string Cashier { get; set; }
    }
}
