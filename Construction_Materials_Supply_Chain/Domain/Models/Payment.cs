using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime AccountingDate { get; set; }
        public string PaymentType { get; set; }
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string TaxCode { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
        public string AmountInWords { get; set; }
        public string PaymentMethod { get; set; }
        public string BankAccountFrom { get; set; }
        public string BankAccountTo { get; set; }
        public string LinkedInvoiceIds { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public string RequestedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string PaidBy { get; set; }
        public string Recipient { get; set; }
        public string AttachmentFile { get; set; }
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
    }

    public class PaymentInvoice
    {
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }

        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}
