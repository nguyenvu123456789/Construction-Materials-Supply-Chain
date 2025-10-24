namespace Domain.Models
{
    public class Driver
    {
        public int DriverId { get; set; }
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public bool Active { get; set; } = true;
        public int PartnerId { get; set; }
        public virtual Partner Partner { get; set; } = null!;
        public virtual ICollection<Transport> Transports { get; set; } = new List<Transport>();
    }
}
