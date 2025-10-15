namespace Domain.Models
{
    public class MoneyAccount
    {
        public int MoneyAccountId { get; set; }
        public string Name { get; set; } = default!;
        public string Type { get; set; } = "Cash"; // "Cash"|"Bank"
        public string? Number { get; set; }        // số tài khoản nếu Bank
        public bool IsActive { get; set; } = true;
    }
}
