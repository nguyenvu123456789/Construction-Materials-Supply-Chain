namespace Domain.Models
{
    public class Porter
    {
        public int PorterId { get; set; }
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public bool Active { get; set; } = true;
        public int PartnerId { get; set; }
        public virtual Partner Partner { get; set; } = null!;
        public virtual ICollection<TransportPorter> TransportPorters { get; set; } = new List<TransportPorter>();
    }
}
