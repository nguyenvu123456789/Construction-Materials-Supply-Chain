public class CreateExportReportDto
{
    public int ExportId { get; set; }
    public int ReportedBy { get; set; }
    public string? Notes { get; set; }
    public List<ExportReportDetailDto> Details { get; set; } = new();
}

public class ExportReportDetailDto
{
    public int MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = null!;
    public bool Keep { get; set; } = false;
}
public class ReviewExportReportDto
{
    public bool Approve { get; set; } = true;
    public int DecidedBy { get; set; }
}
public class ExportReportResponseDto
{
    public int ExportReportId { get; set; }
    public int ExportId { get; set; }
    public DateTime ReportDate { get; set; }
    public int ReportedBy { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }
    public DateTime? DecidedAt { get; set; }
    public int? DecidedBy { get; set; }
    public List<ExportReportDetailResponseDto> Details { get; set; } = new();
}

public class ExportReportDetailResponseDto
{
    public int MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = null!;
    public bool Keep { get; set; }
}
