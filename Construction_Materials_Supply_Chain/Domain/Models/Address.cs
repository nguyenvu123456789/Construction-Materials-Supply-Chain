namespace Domain.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string Name { get; set; } = default!;
        public string? Line1 { get; set; }
        public string? City { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
