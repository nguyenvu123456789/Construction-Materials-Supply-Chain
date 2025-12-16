public class Receipt
{
    public int Id { get; set; }
    public string ReceiptNumber { get; set; }
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
