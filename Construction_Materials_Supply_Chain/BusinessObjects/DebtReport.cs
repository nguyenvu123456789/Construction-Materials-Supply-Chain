namespace BusinessObjects;

public partial class DebtReport
{
    public int DebtReportId { get; set; }

    public string? CustomerName { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly? DueDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
