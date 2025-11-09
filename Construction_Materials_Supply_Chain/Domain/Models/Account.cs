namespace Domain.Models
{
    public class GlAccount
    {
        public int AccountId { get; set; }
        public int PartnerId { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public bool IsPosting { get; set; } = true;
        public int? ParentId { get; set; }
        public GlAccount? Parent { get; set; }
        public ICollection<GlAccount> Children { get; set; } = new List<GlAccount>();
    }
}
