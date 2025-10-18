namespace Application.DTOs
{
    public class AddressCreateDto
    {
        public string Name { get; set; } = default!;
        public string? Line1 { get; set; }
        public string? City { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class AddressUpdateDto
    {
        public string Name { get; set; } = default!;
        public string? Line1 { get; set; }
        public string? City { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class AddressResponseDto
    {
        public int AddressId { get; set; }
        public string Name { get; set; } = default!;
        public string? Line1 { get; set; }
        public string? City { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
