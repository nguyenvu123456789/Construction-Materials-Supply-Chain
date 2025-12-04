namespace Domain.Models
{
    public class MoneyAccount
    {
        public int MoneyAccountId { get; set; }
        public int PartnerId { get; set; }
        public string Name { get; set; } = default!;
        public string Type { get; set; } = "Cash";
        public string? Number { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
