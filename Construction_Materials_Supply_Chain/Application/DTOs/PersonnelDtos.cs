namespace Application.DTOs
{
    public class PersonCreateDto
    {
        public string Type { get; set; } = default!;
        public int PartnerId { get; set; }

        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public bool Active { get; set; } = true;

        public int? BirthYear { get; set; }
        public string? Hometown { get; set; }

        public string? LicenseClass { get; set; }
        public string? LicenseNumber { get; set; }

        public string? Code { get; set; }
        public string? PlateNumber { get; set; }
        public string? VehicleClass { get; set; }
        public decimal? CapacityTon { get; set; }
        public string? MinLicenseClass { get; set; }
    }

    public class PersonUpdateDto
    {
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public bool Active { get; set; } = true;

        public int? BirthYear { get; set; }
        public string? Hometown { get; set; }

        public string? LicenseClass { get; set; }
        public string? LicenseNumber { get; set; }

        public string? Code { get; set; }
        public string? PlateNumber { get; set; }
        public string? VehicleClass { get; set; }
        public decimal? CapacityTon { get; set; }
        public string? MinLicenseClass { get; set; }
    }

    public class PersonResponseDto
    {
        public string Type { get; set; } = default!;
        public int Id { get; set; }
        public int PartnerId { get; set; }

        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public bool Active { get; set; }

        public int? BirthYear { get; set; }
        public string? Hometown { get; set; }

        public string? LicenseClass { get; set; }
        public string? LicenseNumber { get; set; }

        public string? Code { get; set; }
        public string? PlateNumber { get; set; }
        public string? VehicleClass { get; set; }
        public decimal? CapacityTon { get; set; }
        public string? MinLicenseClass { get; set; }
    }

    public class AvailabilityItemDto
    {
        public string Type { get; set; } = default!;
        public int Id { get; set; }
        public string NameOrCode { get; set; } = default!;
        public string? Plate { get; set; }
        public bool FreeNow { get; set; }
        public DateTimeOffset? AvailableAt { get; set; }
    }

    public class AvailabilityResponseDto
    {
        public List<AvailabilityItemDto> Free { get; set; } = new();
        public List<AvailabilityItemDto> Busy { get; set; } = new();
    }
}
