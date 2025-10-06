namespace Domain;

public partial class FinancialReport
{
    public int ReportId { get; set; }

    public string? ReportType { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
