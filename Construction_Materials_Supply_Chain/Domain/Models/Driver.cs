namespace Domain.Models
{
    public class Driver
    {
        public int DriverId { get; set; }
        public int PartnerId { get; set; }
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Hometown { get; set; }
        public string LicenseClass { get; set; } = "B";
        public bool Active { get; set; } = true;

        public virtual Partner Partner { get; set; } = default!;
        public virtual ICollection<TransportAssignment> TransportAssignments { get; set; } = new List<TransportAssignment>();
    }
}
