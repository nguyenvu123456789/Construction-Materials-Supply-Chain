// Request tạo báo cáo
public class CreateImportReportDto
{
    public string InvoiceCode { get; set; } = null!;
    public int CreatedBy { get; set; }
    public string? Notes { get; set; }
    public List<ImportReportDetailDto> Details { get; set; } = new();
}

public class ImportReportDetailDto
{
    public int MaterialId { get; set; }
    public int TotalQuantity { get; set; }
    public int GoodQuantity { get; set; }
    public int DamagedQuantity { get; set; }
    public string? Comment { get; set; }
}

// Request duyệt báo cáo
public class ReviewImportReportDto
{
    public string Status { get; set; } = "Approved"; // Approved / Rejected
    public int ReviewedBy { get; set; }
    public string? RejectReason { get; set; }
}
