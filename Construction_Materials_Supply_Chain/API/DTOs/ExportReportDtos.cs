namespace API.DTOs
{
    public class ExportReportDetailDto
    {
        public int MaterialId { get; set; }
        public decimal Quantity { get; set; }
        public string Reason { get; set; } = null!;
        public bool Keep { get; set; }
    }

    public class ExportReportCreateDto
    {
        public int ExportId { get; set; }
        public int ReportedBy { get; set; }
        public string? Notes { get; set; }
        public List<ExportReportDetailDto> Details { get; set; } = new();
    }
}
