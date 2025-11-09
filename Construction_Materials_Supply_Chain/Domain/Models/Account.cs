namespace Domain.Models
{
    public class GlAccount
    {
        public int AccountId { get; set; }
        public int PartnerId { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsPosting { get; set; }
        public int? ParentId { get; set; }
        public GlAccount? Parent { get; set; }
        public ICollection<GlAccount> Children { get; set; } = new List<GlAccount>();
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
