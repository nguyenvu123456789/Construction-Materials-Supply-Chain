namespace Domain.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public string Code { get; set; } = default!;
        public string PlateNumber { get; set; } = default!;
        public string? VehicleClass { get; set; }
        public bool Active { get; set; } = true;
        public int PartnerId { get; set; }
        public virtual Partner Partner { get; set; } = null!;
        public virtual ICollection<Transport> Transports { get; set; } = new List<Transport>();
    }
}
