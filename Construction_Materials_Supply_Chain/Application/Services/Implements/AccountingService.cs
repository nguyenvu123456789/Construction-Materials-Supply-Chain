using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Interface;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Implements
{
    public class AccountingService : IAccountingService
    {
        private readonly IAccountingEntryRepository _entryRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IReceiptRepository _receiptRepo;
        private readonly IPaymentRepository _paymentRepo;

        public AccountingService(
            IAccountingEntryRepository entryRepo,
            IInvoiceRepository invoiceRepo,
            IReceiptRepository receiptRepo,
            IPaymentRepository paymentRepo)
        {
            _entryRepo = entryRepo;
            _invoiceRepo = invoiceRepo;
            _receiptRepo = receiptRepo;
            _paymentRepo = paymentRepo;
        }

        public List<LedgerEntryDto> GetLedger(DateTime from, DateTime to)
        {
            var entries = _entryRepo.GetEntries(from, to);
            var result = new List<LedgerEntryDto>();

            decimal runningBalance = 0;
            int no = 1;

            foreach (var e in entries)
            {
                runningBalance += e.DebitAmount - e.CreditAmount;

                result.Add(new LedgerEntryDto
                {
                    No = no++,
                    DocumentDate = e.EntryDate,
                    PostingDate = e.EntryDate,
                    DocumentType = e.DocumentType,
                    DocumentNumber = e.DocumentNumber,
                    ContraAccount = $"{e.DebitAccount}/{e.CreditAccount}",
                    PartnerName = e.Description,
                    Description = e.Description,
                    Debit = e.DebitAmount,
                    Credit = e.CreditAmount,
                    Balance = Math.Abs(runningBalance),
                    BalanceType = runningBalance >= 0 ? "Nợ" : "Có"
                });
            }

            return result;
        }

        public List<APAgingDto> GetAPAging()
        {
            var invoices = _invoiceRepo.GetAll()
                .Where(i => i.InvoiceType == "Import")
                .ToList();

            var list = new List<APAgingDto>();

            foreach (var inv in invoices)
            {
                var paid = _paymentRepo.GetPaymentsByInvoice(inv.InvoiceCode)
                                       .Sum(x => x.Amount);

                var remaining = inv.PayableAmount - paid;

                int overdue = 0;
                if (inv.DueDate.HasValue)
                    overdue = (DateTime.Now - inv.DueDate.Value).Days;

                list.Add(new APAgingDto
                {
                    InvoiceCode = inv.InvoiceCode,
                    InvoiceDate = inv.IssueDate,
                    DueDate = inv.DueDate,
                    OverdueDays = overdue > 0 ? overdue : 0,
                    OriginalAmount = inv.PayableAmount,
                    PaidAmount = paid,
                    RemainingAmount = remaining,
                    Status = remaining <= 0 ? "Đã thanh toán"
                           : overdue > 0 ? "Quá hạn"
                           : "Chưa đến hạn"
                });
            }

            return list;
        }

        public List<CashBookDto> GetCashBook(DateTime from, DateTime to)
        {
            var receipts = _receiptRepo.GetByDateRange(from, to);
            var payments = _paymentRepo.GetByDateRange(from, to);

            var entries = new List<CashBookDto>();
            int no = 1;
            decimal balance = 0;

            foreach (var r in receipts)
            {
                balance += r.Amount;

                entries.Add(new CashBookDto
                {
                    No = no++,
                    DocumentDate = r.DateCreated,
                    PostingDate = r.DateCreated,
                    Type = "PT",
                    DocumentNumber = r.ReceiptNumber,
                    Partner = r.CreatedBy,
                    ContraAccount = r.DebitAccount,
                    Description = r.Notes,
                    InAmount = r.Amount,
                    OutAmount = 0,
                    Balance = balance,
                    Cashier = r.CreatedBy
                });
            }

            foreach (var p in payments)
            {
                balance -= p.Amount;

                entries.Add(new CashBookDto
                {
                    No = no++,
                    DocumentDate = p.DateCreated,
                    PostingDate = p.DateCreated,
                    Type = "PC",
                    DocumentNumber = p.PaymentNumber,
                    Partner = p.CreatedBy,
                    ContraAccount = p.DebitAccount,
                    Description = p.Notes,
                    InAmount = 0,
                    OutAmount = p.Amount,
                    Balance = balance,
                    Cashier = p.CreatedBy
                });
            }

            return entries
                .OrderBy(x => x.DocumentDate)
                .ThenBy(x => x.No)
                .ToList();
        }
    }
}
