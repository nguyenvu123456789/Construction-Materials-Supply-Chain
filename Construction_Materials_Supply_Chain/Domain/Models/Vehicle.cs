namespace Domain.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public int PartnerId { get; set; }
        public string Code { get; set; } = default!;
        public string PlateNumber { get; set; } = default!;
        public string? VehicleClass { get; set; }
        public string MinLicenseClass { get; set; } = "B";
        public decimal? PayloadTons { get; set; }
        public bool Active { get; set; } = true;

        public virtual Partner Partner { get; set; } = default!;
        public virtual ICollection<Transport> Transports { get; set; } = new List<Transport>();
    }
}
