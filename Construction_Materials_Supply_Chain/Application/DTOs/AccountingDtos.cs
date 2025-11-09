namespace Application.DTOs
{
    public class PostResultDto
    {
        public bool Ok { get; set; }
        public string Type { get; set; } = "";
        public int Id { get; set; }
    }

    public class LedgerLineDto
    {
        public DateTime PostingDate { get; set; }
        public string SourceType { get; set; } = "";
        public int SourceId { get; set; }
        public string? ReferenceNo { get; set; }
        public int? PartnerId { get; set; }
        public int? InvoiceId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

    public class GeneralLedgerResponseDto
    {
        public object Account { get; set; } = default!;
        public object Period { get; set; } = default!;
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public List<LedgerLineDto> Entries { get; set; } = new();
    }

    public class AgingItemDto
    {
        public int PartnerId { get; set; }
        public int? InvoiceId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Outstanding { get; set; }
    }

    public class AgingResponseDto
    {
        public DateTime AsOf { get; set; }
        public List<AgingItemDto> Items { get; set; } = new();
    }

    public class CashbookItemDto
    {
        public string Type { get; set; } = "";
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "";
        public int? PartnerId { get; set; }
        public int? InvoiceId { get; set; }
        public string? Reference { get; set; }
        public int Id { get; set; }
    }

    public class CashbookResponseDto
    {
        public object Period { get; set; } = default!;
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal Net { get; set; }
        public List<CashbookItemDto> Items { get; set; } = new();
    }

    public class BankReconLineDto
    {
        public int BankStatementLineId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
        public string? ExternalRef { get; set; }
        public string Status { get; set; } = "";
        public int? ReceiptId { get; set; }
        public int? PaymentId { get; set; }
    }

    public class BankReconResponseDto
    {
        public object Statement { get; set; } = default!;
        public decimal StatementAmount { get; set; }
        public decimal BookNet { get; set; }
        public List<BankReconLineDto> Matched { get; set; } = new();
        public List<BankReconLineDto> Unmatched { get; set; } = new();
    }

    public class GlAccountCreateDto
    {
        public int PartnerId { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsPosting { get; set; } = true;
        public int? ParentId { get; set; }
    }

    public class GlAccountUpdateDto
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsPosting { get; set; }
        public int? ParentId { get; set; }
    }

    public class GlAccountDto
    {
        public int AccountId { get; set; }
        public int PartnerId { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsPosting { get; set; }
        public int? ParentId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
