using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class BankStatement
    {
        public int BankStatementId { get; set; }
        public int MoneyAccountId { get; set; }
        public MoneyAccount MoneyAccount { get; set; } = default!;
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public ICollection<BankStatementLine> Lines { get; set; } = new List<BankStatementLine>();
    }

    public class BankStatementLine
    {
        public int BankStatementLineId { get; set; }
        public int BankStatementId { get; set; }
        public BankStatement BankStatement { get; set; } = default!;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }           // + in, - out
        public string Description { get; set; } = default!;
        public string? ExternalRef { get; set; }
        public string Status { get; set; } = "Unreconciled"; // Unreconciled|Reconciled
        public int? ReceiptId { get; set; }
        public int? PaymentId { get; set; }
    }
}
