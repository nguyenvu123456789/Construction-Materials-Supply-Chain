using Application.DTOs;
using Domain.Interface;
using Domain.Interface.Base;
using Domain.Interfaces;
using Domain.Models;

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
        private readonly IExportRepository _exportRepo;

        public AccountingPostingService(
            IJournalEntryRepository jeRepo,
            IPostingPolicyRepository policyRepo,
            ISubLedgerRepository subRepo,
            IGenericRepository<Receipt> receiptRepo,
            IGenericRepository<Payment> paymentRepo,
            IInvoiceRepository invoiceRepo,
            IExportRepository exportRepo)
        {
            _jeRepo = jeRepo;
            _policyRepo = policyRepo;
            _subRepo = subRepo;
            _receiptRepo = receiptRepo;
            _paymentRepo = paymentRepo;
            _invoiceRepo = invoiceRepo;
            _exportRepo = exportRepo;
        }

        public PostResultDto PostSalesInvoice(int invoiceId)
        {
            if (_jeRepo.QueryFull().Any(j => j.SourceType == "SalesInvoice" && j.SourceId == invoiceId))
                return new PostResultDto { Ok = true, Type = "SalesInvoice", Id = invoiceId };

            var inv = _invoiceRepo.GetWithDetails(invoiceId);
            var subtotal = inv.InvoiceDetails.Sum(d => (d.LineTotal ?? (d.Quantity * d.UnitPrice)));
            var total = inv.TotalAmount;
            var vat = total > subtotal ? total - subtotal : 0m;
            var policies = _policyRepo.QueryByDoc("SalesInvoice").ToList();
            var je = new JournalEntry
            {
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
            var subtotal = inv.InvoiceDetails.Sum(d => (d.LineTotal ?? (d.Quantity * d.UnitPrice)));
            var total = inv.TotalAmount;
            var vat = total > subtotal ? total - subtotal : 0m;
            var policies = _policyRepo.QueryByDoc("PurchaseInvoice").ToList();
            var je = new JournalEntry
            {
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

        public PostResultDto PostExportCogs(int exportId)
        {
            if (_jeRepo.QueryFull().Any(j => j.SourceType == "ExportCOGS" && j.SourceId == exportId))
                return new PostResultDto { Ok = true, Type = "ExportCOGS", Id = exportId };

            var ex = _exportRepo.GetWithDetails(exportId);
            var cogs = ex.ExportDetails.Sum(d => d.Quantity * d.UnitPrice);
            var policies = _policyRepo.QueryByDoc("ExportCOGS").ToList();
            var je = new JournalEntry
            {
                PostingDate = ex.ExportDate,
                SourceType = "ExportCOGS",
                SourceId = ex.ExportId,
                ReferenceNo = ex.ExportCode,
                Memo = "COGS posting"
            };
            AddByPolicy(je, policies, "COGS", cogs, null, null, false);
            _jeRepo.Add(je);
            return new PostResultDto { Ok = true, Type = "ExportCOGS", Id = exportId };
        }

        public PostResultDto PostReceipt(int receiptId)
        {
            if (_jeRepo.QueryFull().Any(j => j.SourceType == "Receipt" && j.SourceId == receiptId))
                return new PostResultDto { Ok = true, Type = "Receipt", Id = receiptId };

            var r = _receiptRepo.GetById(receiptId);
            var policies = _policyRepo.QueryByDoc("Receipt").ToList();
            var rule = r.Method == "Cash" ? "Cash" : "Bank";
            var je = new JournalEntry
            {
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
            var policies = _policyRepo.QueryByDoc("Payment").ToList();
            var rule = p.Method == "Cash" ? "Cash" : "Bank";
            var je = new JournalEntry
            {
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

        private void AddByPolicy(JournalEntry je, List<PostingPolicy> policies, string ruleKey, decimal amount, int? partnerId, int? invoiceId, bool reverse)
        {
            var pol = policies.First(x => x.RuleKey == ruleKey);
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
