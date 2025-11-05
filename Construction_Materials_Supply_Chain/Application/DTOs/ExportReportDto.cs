using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class CreateExportReportDetailDto
    {
        public int MaterialId { get; set; }
        public decimal QuantityDamaged { get; set; }
        public string Reason { get; set; } = null!;
    }

    public class CreateExportReportDto
    {
        public int ExportId { get; set; }
        public int ReportedBy { get; set; }
        public string? Notes { get; set; }
        public List<CreateExportReportDetailDto> Details { get; set; } = new();
    }

    public class ReviewExportReportDto
    {
        public int DecidedBy { get; set; }
        public string? Notes { get; set; }
        public bool? Approve { get; set; }
        public List<ReviewExportReportDetailDto>? Details { get; set; }
    }
    public class ReviewExportReportDetailDto
    {
        public int MaterialId { get; set; }
        public bool Keep { get; set; }
    }

    public class ExportReportResponseDto
    {
        public int ExportReportId { get; set; }
        public int ExportId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime ReportDate { get; set; }
        public int ReportedBy { get; set; }
        public string? Notes { get; set; }
        public List<ExportReportDetailResponseDto> Details { get; set; } = new();
        public List<HandleRequestDto> HandleHistory { get; set; } = new();
    }

    public class ExportReportDetailResponseDto
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = null!;
        public decimal QuantityDamaged { get; set; }
        public string Reason { get; set; } = null!;
        [JsonIgnore]
        public bool Keep { get; set; }
    }
}
