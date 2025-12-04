namespace Domain.Models
{
    public class PostingPolicy
    {
        public int PostingPolicyId { get; set; }
        public int PartnerId { get; set; }
        public string DocumentType { get; set; } = default!;
        public string RuleKey { get; set; } = default!;
        public int DebitAccountId { get; set; }
        public int CreditAccountId { get; set; }
        public string? PartnerType { get; set; }
        public string? MaterialCategory { get; set; }
    }
}
