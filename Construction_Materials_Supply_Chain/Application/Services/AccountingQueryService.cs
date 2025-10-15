using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface.Base;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class AccountingQueryService : IAccountingQueryService
    {
        private readonly IJournalEntryRepository _jeRepo;
        private readonly ISubLedgerRepository _subRepo;
        private readonly IAccountRepository _accRepo;
        private readonly IGenericRepository<Receipt> _receiptRepo;
        private readonly IGenericRepository<Payment> _paymentRepo;
        private readonly IGenericRepository<BankStatement> _bsRepo;
        private readonly IGenericRepository<BankStatementLine> _bslRepo;
        private readonly IMapper _mapper;

        public AccountingQueryService(
            IJournalEntryRepository jeRepo,
            ISubLedgerRepository subRepo,
            IAccountRepository accRepo,
            IGenericRepository<Receipt> receiptRepo,
            IGenericRepository<Payment> paymentRepo,
            IGenericRepository<BankStatement> bsRepo,
            IGenericRepository<BankStatementLine> bslRepo,
            IMapper mapper)
        {
            _jeRepo = jeRepo;
            _subRepo = subRepo;
            _accRepo = accRepo;
            _receiptRepo = receiptRepo;
            _paymentRepo = paymentRepo;
            _bsRepo = bsRepo;
            _bslRepo = bslRepo;
            _mapper = mapper;
        }

        public GeneralLedgerResponseDto GetGeneralLedger(DateTime from, DateTime to, string accountCode)
        {
            var acc = _accRepo.GetAll().FirstOrDefault(a => a.Code == accountCode);
            if (acc == null)
                return new GeneralLedgerResponseDto { Account = new { Code = accountCode, NotFound = true }, Period = new { from, to } };

            var lines = _jeRepo.QueryFull()
                               .Where(j => j.PostingDate.Date >= from.Date && j.PostingDate.Date <= to.Date)
                               .SelectMany(j => j.Lines.Where(l => l.AccountId == acc.AccountId))
                               .ToList();

            var dtoLines = _mapper.Map<List<LedgerLineDto>>(lines);
            var totalDebit = dtoLines.Sum(x => x.Debit);
            var totalCredit = dtoLines.Sum(x => x.Credit);

            return new GeneralLedgerResponseDto
            {
                Account = new { acc.AccountId, acc.Code, acc.Name },
                Period = new { from, to },
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                Entries = dtoLines
            };
        }

        public AgingResponseDto GetARAging(DateTime asOf)
        {
            var items = _subRepo.GetAll()
                                .Where(x => x.SubLedgerType == "AR" && x.Date <= asOf)
                                .GroupBy(x => new { x.PartnerId, x.InvoiceId })
                                .Select(g => new AgingItemDto
                                {
                                    PartnerId = g.Key.PartnerId,
                                    InvoiceId = g.Key.InvoiceId,
                                    Debit = g.Sum(x => x.Debit),
                                    Credit = g.Sum(x => x.Credit),
                                    Outstanding = g.Sum(x => x.Debit - x.Credit)
                                })
                                .Where(x => x.Outstanding != 0)
                                .ToList();

            return new AgingResponseDto { AsOf = asOf, Items = items };
        }

        public AgingResponseDto GetAPAging(DateTime asOf)
        {
            var items = _subRepo.GetAll()
                                .Where(x => x.SubLedgerType == "AP" && x.Date <= asOf)
                                .GroupBy(x => new { x.PartnerId, x.InvoiceId })
                                .Select(g => new AgingItemDto
                                {
                                    PartnerId = g.Key.PartnerId,
                                    InvoiceId = g.Key.InvoiceId,
                                    Debit = g.Sum(x => x.Debit),
                                    Credit = g.Sum(x => x.Credit),
                                    Outstanding = g.Sum(x => x.Credit - x.Debit)
                                })
                                .Where(x => x.Outstanding != 0)
                                .ToList();

            return new AgingResponseDto { AsOf = asOf, Items = items };
        }

        public CashbookResponseDto GetCashbook(DateTime from, DateTime to, string? method)
        {
            method = string.IsNullOrWhiteSpace(method) ? "" : method;

            var receiptEntities = _receiptRepo.GetAll()
                .Where(r => r.Date.Date >= from.Date && r.Date.Date <= to.Date && (method == "" || r.Method == method))
                .ToList();

            var paymentEntities = _paymentRepo.GetAll()
                .Where(p => p.Date.Date >= from.Date && p.Date.Date <= to.Date && (method == "" || p.Method == method))
                .ToList();

            var receipts = _mapper.Map<List<CashbookItemDto>>(receiptEntities);
            var payments = _mapper.Map<List<CashbookItemDto>>(paymentEntities);

            var items = receipts.Concat(payments).OrderBy(x => x.Date).ToList();
            var totalIn = receipts.Sum(x => x.Amount);
            var totalOut = payments.Sum(x => -x.Amount);
            var net = totalIn - totalOut;

            return new CashbookResponseDto
            {
                Period = new { from, to, method = method == "" ? "All" : method },
                TotalIn = totalIn,
                TotalOut = totalOut,
                Net = net,
                Items = items
            };
        }

        public BankReconResponseDto GetBankReconciliation(int statementId)
        {
            var st = _bsRepo.GetById(statementId);
            if (st == null) return new BankReconResponseDto { Statement = new { NotFound = true, Id = statementId } };

            var lines = _bslRepo.GetAll().Where(l => l.BankStatementId == statementId).ToList();
            var matched = lines.Where(l => l.Status == "Reconciled").ToList();
            var unmatched = lines.Where(l => l.Status != "Reconciled").ToList();

            return new BankReconResponseDto
            {
                Statement = new { st.BankStatementId, st.MoneyAccountId, st.From, st.To },
                StatementAmount = lines.Sum(l => l.Amount),
                BookNet = _receiptRepo.GetAll().Where(r => r.Method == "Bank" && r.Date >= st.From && r.Date <= st.To).Sum(r => r.Amount)
                         - _paymentRepo.GetAll().Where(p => p.Method == "Bank" && p.Date >= st.From && p.Date <= st.To).Sum(p => p.Amount),
                Matched = _mapper.Map<List<BankReconLineDto>>(matched),
                Unmatched = _mapper.Map<List<BankReconLineDto>>(unmatched)
            };
        }
    }
}
