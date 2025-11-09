using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Interface.Base;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services
{
    public class AccountingPostingService : IAccountingPostingService
    {
        private readonly IJournalEntryRepository _jeRepo;
        private readonly IPostingPolicyRepository _policyRepo;
        private readonly ISubLedgerRepository _subRepo;
        private readonly IGenericRepository<Receipt> _receiptRepo;
        private readonly IGenericRepository<Payment> _paymentRepo;
        private readonly IInvoiceRepository _invoiceRepo;

        public AccountingPostingService(
            IJournalEntryRepository jeRepo,
            IPostingPolicyRepository policyRepo,
            ISubLedgerRepository subRepo,
            IGenericRepository<Receipt> receiptRepo,
            IGenericRepository<Payment> paymentRepo,
            IInvoiceRepository invoiceRepo)
        {
            _jeRepo = jeRepo;
            _policyRepo = policyRepo;
            _subRepo = subRepo;
            _receiptRepo = receiptRepo;
            _paymentRepo = paymentRepo;
            _invoiceRepo = invoiceRepo;
        }

        public PostResultDto PostSalesInvoice(int invoiceId)
        {
            if (_jeRepo.QueryFull().Any(j => j.SourceType == "SalesInvoice" && j.SourceId == invoiceId))
                return new PostResultDto { Ok = true, Type = "SalesInvoice", Id = invoiceId };

            var inv = _invoiceRepo.GetWithDetails(invoiceId);
            if (inv is null || inv.InvoiceType != "Export" || !CanPostInvoice(inv))
                return new PostResultDto { Ok = false, Type = "SalesInvoice", Id = invoiceId };

            var subtotal = inv.InvoiceDetails.Sum(d => (d.LineTotal ?? (d.Quantity * d.UnitPrice)));
            var total = inv.TotalAmount;
            var vat = total > subtotal ? total - subtotal : 0m;

            var policies = Policies(inv.PartnerId, "SalesInvoice");

            var je = new JournalEntry
            {
                PartnerId = inv.PartnerId,
                PostingDate = inv.IssueDate,
                SourceType = "SalesInvoice",
                SourceId = inv.InvoiceId,
                ReferenceNo = inv.InvoiceCode,
                Memo = "Sales invoice"
            };

            AddByPolicy(je, policies, "Revenue", subtotal, inv.PartnerId, inv.InvoiceId, false);
            if (vat > 0) AddByPolicy(je, policies, "VATOut", vat, inv.PartnerId, inv.InvoiceId, false);

            _jeRepo.Add(je);

            _subRepo.Add(new SubLedgerEntry
            {
                SubLedgerType = "AR",
                PartnerId = inv.PartnerId,
                InvoiceId = inv.InvoiceId,
                Date = inv.IssueDate,
                Debit = total,
                Credit = 0m,
                Reference = "SalesInvoice#" + inv.InvoiceId
            });

            return new PostResultDto { Ok = true, Type = "SalesInvoice", Id = invoiceId };
        }

        public PostResultDto PostPurchaseInvoice(int invoiceId)
        {
            if (_jeRepo.QueryFull().Any(j => j.SourceType == "PurchaseInvoice" && j.SourceId == invoiceId))
                return new PostResultDto { Ok = true, Type = "PurchaseInvoice", Id = invoiceId };

            var inv = _invoiceRepo.GetWithDetails(invoiceId);
            if (inv is null || inv.InvoiceType != "Import" || !CanPostInvoice(inv))
                return new PostResultDto { Ok = false, Type = "PurchaseInvoice", Id = invoiceId };

            var subtotal = inv.InvoiceDetails.Sum(d => (d.LineTotal ?? (d.Quantity * d.UnitPrice)));
            var total = inv.TotalAmount;
            var vat = total > subtotal ? total - subtotal : 0m;

            var policies = Policies(inv.PartnerId, "PurchaseInvoice");

            var je = new JournalEntry
            {
                PartnerId = inv.PartnerId,
                PostingDate = inv.IssueDate,
                SourceType = "PurchaseInvoice",
                SourceId = inv.InvoiceId,
                ReferenceNo = inv.InvoiceCode,
                Memo = "Purchase invoice"
            };

            AddByPolicy(je, policies, "Inventory", subtotal, inv.PartnerId, inv.InvoiceId, false);
            if (vat > 0) AddByPolicy(je, policies, "VATIn", vat, inv.PartnerId, inv.InvoiceId, false);

            _jeRepo.Add(je);

            _subRepo.Add(new SubLedgerEntry
            {
                SubLedgerType = "AP",
                PartnerId = inv.PartnerId,
                InvoiceId = inv.InvoiceId,
                Date = inv.IssueDate,
                Debit = 0m,
                Credit = total,
                Reference = "PurchaseInvoice#" + inv.InvoiceId
            });

            return new PostResultDto { Ok = true, Type = "PurchaseInvoice", Id = invoiceId };
        }

        public PostResultDto PostReceipt(int receiptId)
        {
            if (_jeRepo.QueryFull().Any(j => j.SourceType == "Receipt" && j.SourceId == receiptId))
                return new PostResultDto { Ok = true, Type = "Receipt", Id = receiptId };

            var r = _receiptRepo.GetById(receiptId);
            var rule = r.Method == "Cash" ? "Cash" : "Bank";
            var policies = Policies(r.PartnerId ?? 0, "Receipt");

            var je = new JournalEntry
            {
                PartnerId = r.PartnerId ?? 0,
                PostingDate = r.Date,
                SourceType = "Receipt",
                SourceId = r.ReceiptId,
                ReferenceNo = r.Reference,
                Memo = "Receipt"
            };

            AddByPolicy(je, policies, rule, r.Amount, r.PartnerId, r.InvoiceId, false);
            _jeRepo.Add(je);

            if (r.PartnerId.HasValue)
            {
                _subRepo.Add(new SubLedgerEntry
                {
                    SubLedgerType = "AR",
                    PartnerId = r.PartnerId.Value,
                    InvoiceId = r.InvoiceId,
                    Date = r.Date,
                    Debit = 0m,
                    Credit = r.Amount,
                    Reference = "Receipt#" + r.ReceiptId
                });
            }

            return new PostResultDto { Ok = true, Type = "Receipt", Id = receiptId };
        }

        public PostResultDto PostPayment(int paymentId)
        {
            if (_jeRepo.QueryFull().Any(j => j.SourceType == "Payment" && j.SourceId == paymentId))
                return new PostResultDto { Ok = true, Type = "Payment", Id = paymentId };

            var p = _paymentRepo.GetById(paymentId);
            var rule = p.Method == "Cash" ? "Cash" : "Bank";
            var policies = Policies(p.PartnerId ?? 0, "Payment");

            var je = new JournalEntry
            {
                PartnerId = p.PartnerId ?? 0,
                PostingDate = p.Date,
                SourceType = "Payment",
                SourceId = p.PaymentId,
                ReferenceNo = p.Reference,
                Memo = "Payment"
            };

            AddByPolicy(je, policies, rule, p.Amount, p.PartnerId, p.InvoiceId, true);
            _jeRepo.Add(je);

            if (p.PartnerId.HasValue)
            {
                _subRepo.Add(new SubLedgerEntry
                {
                    SubLedgerType = "AP",
                    PartnerId = p.PartnerId.Value,
                    InvoiceId = p.InvoiceId,
                    Date = p.Date,
                    Debit = p.Amount,
                    Credit = 0m,
                    Reference = "Payment#" + p.PaymentId
                });
            }

            return new PostResultDto { Ok = true, Type = "Payment", Id = paymentId };
        }

        public PostResultDto Unpost(string sourceType, int sourceId)
        {
            var jes = _jeRepo.QueryFull().Where(j => j.SourceType == sourceType && j.SourceId == sourceId).ToList();
            if (jes.Count == 0) return new PostResultDto { Ok = true, Type = "Unpost", Id = sourceId };

            var refKey = sourceType + "#" + sourceId;
            var subs = _subRepo.GetAll().Where(s => s.Reference == refKey).ToList();

            foreach (var s in subs) _subRepo.Delete(s);
            foreach (var j in jes) _jeRepo.Delete(j);

            return new PostResultDto { Ok = true, Type = "Unpost", Id = sourceId };
        }

        private static bool CanPostInvoice(Invoice inv)
        {
            if (inv is null) return false;

            if (inv.InvoiceType == "Export")
                return inv.ExportStatus == "Success";

            if (inv.InvoiceType == "Import")
                return inv.ImportStatus == "Success";

            return false;
        }

        private List<PostingPolicy> Policies(int partnerId, string documentType)
        {
            var list = _policyRepo.QueryByDoc(partnerId, documentType).ToList();
            if (list.Count == 0) list = _policyRepo.QueryByDoc(documentType).ToList();
            return list;
        }

        private void AddByPolicy(JournalEntry je, List<PostingPolicy> policies, string ruleKey, decimal amount, int? partnerId, int? invoiceId, bool reverse)
        {
            var pol = policies.FirstOrDefault(x => x.RuleKey == ruleKey);
            if (pol == null) throw new InvalidOperationException($"PostingPolicy not found for RuleKey={ruleKey}");
            var dr = reverse ? pol.CreditAccountId : pol.DebitAccountId;
            var cr = reverse ? pol.DebitAccountId : pol.CreditAccountId;

            je.Lines.Add(new JournalLine { AccountId = dr, Debit = amount, Credit = 0m, PartnerId = partnerId, InvoiceId = invoiceId });
            je.Lines.Add(new JournalLine { AccountId = cr, Debit = 0m, Credit = amount, PartnerId = partnerId, InvoiceId = invoiceId });
        }

        public PostResultDto PostSalesInvoiceByCode(string invoiceCode)
        {
            var inv = _invoiceRepo.GetWithDetailsByCode(invoiceCode);
            if (inv == null) return new PostResultDto { Ok = false, Type = "SalesInvoice", Id = 0 };
            if (_jeRepo.QueryFull().Any(j => j.SourceType == "SalesInvoice" && j.SourceId == inv.InvoiceId))
                return new PostResultDto { Ok = true, Type = "SalesInvoice", Id = inv.InvoiceId };
            return PostSalesInvoice(inv.InvoiceId);
        }
    }
}
