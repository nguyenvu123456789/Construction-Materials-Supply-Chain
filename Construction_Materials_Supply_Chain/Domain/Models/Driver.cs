namespace Domain.Models
{
    public class Driver
    {
        public int DriverId { get; set; }
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public bool Active { get; set; } = true;
    }
}
