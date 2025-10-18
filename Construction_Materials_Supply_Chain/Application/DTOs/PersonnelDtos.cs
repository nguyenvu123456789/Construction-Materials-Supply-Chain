namespace Application.DTOs
{
    public class PersonCreateDto
    {
        public string Type { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public bool Active { get; set; } = true
;
        public string? Code { get; set; }
        public string? PlateNumber { get; set; }
        public string? VehicleClass { get; set; }
    }

    public class PersonUpdateDto
    {
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public bool Active { get; set; } = true;
        public string? Code { get; set; }
        public string? PlateNumber { get; set; }
        public string? VehicleClass { get; set; }
    }

    public class PersonResponseDto
    {
        public string Type { get; set; } = default!;
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public bool Active { get; set; }
        public string? Code { get; set; }
        public string? PlateNumber { get; set; }
        public string? VehicleClass { get; set; }
    }
}
