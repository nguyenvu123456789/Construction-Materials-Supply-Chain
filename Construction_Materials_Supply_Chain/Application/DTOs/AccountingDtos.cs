using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class ReceiptDto
    {
        public int ReceiptId { get; set; }
        public string ReceiptNumber { get; set; }
        public decimal Amount { get; set; }
        public string AmountInWords { get; set; }
        public string CustomerName { get; set; }
        public string PaymentMethod { get; set; }
        public string? BankAccount { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public List<int> InvoiceIds { get; set; }
    }

    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public string PaymentNumber { get; set; }
        public decimal Amount { get; set; }
        public string AmountInWords { get; set; }
        public string VendorName { get; set; }
        public string PaymentMethod { get; set; }
        public string? BankAccount { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public List<int> InvoiceIds { get; set; }
    }

    public class ReceiptCreateDto
    {
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string? BankAccount { get; set; }
        public string Reference { get; set; }
        public List<int> InvoiceIds { get; set; }
        public string CreatedBy { get; set; }
    }

    public class PaymentCreateDto
    {
        public string VendorName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string? BankAccount { get; set; }
        public string Reference { get; set; }
        public List<int> InvoiceIds { get; set; }
        public string CreatedBy { get; set; }
    }
}
