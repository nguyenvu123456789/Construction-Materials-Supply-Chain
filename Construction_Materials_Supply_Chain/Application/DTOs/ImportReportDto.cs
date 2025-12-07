using Application.DTOs;

public class CreateImportReportDetailDto
{
    public int MaterialId { get; set; }
    public int TotalQuantity { get; set; }
    public int GoodQuantity { get; set; }
    public int DamagedQuantity { get; set; }
    public string? Comment { get; set; }
}

public class CreateImportReportDto
{
    public string InvoiceCode { get; set; } = null!;
    public int CreatedBy { get; set; }
    public string? Notes { get; set; }
    public List<CreateImportReportDetailDto> Details { get; set; } = new();
}


public class ImportReportResponseDto
{
    public int ImportReportId { get; set; }
    public string ImportReportCode { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectReason { get; set; }
    public int CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public SimpleImportDto Import { get; set; } = null!;
    public SimpleInvoiceDto Invoice { get; set; } = null!;
    public List<ImportReportDetailDto> Details { get; set; } = new();
    public List<HandleRequestDto>? HandleHistory { get; set; }
}

public class SimpleImportDto
{
    public int ImportId { get; set; }
    public string ImportCode { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = null!;
}

public class SimpleInvoiceDto
{
    public int InvoiceId { get; set; }
    public string InvoiceCode { get; set; } = null!;
    public string InvoiceType { get; set; } = null!;
    public DateTime IssueDate { get; set; }
}

public class ImportReportDetailDto
{
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = null!;
    public string MaterialName { get; set; } = null!;
    public int TotalQuantity { get; set; }
    public int GoodQuantity { get; set; }
    public int DamagedQuantity { get; set; }
    public string? Comment { get; set; }
}

public class ReviewImportReportDto
{
    public string Status { get; set; } = "Approved"; // Approved / Rejected
    public int ReviewedBy { get; set; }
    public string? Note { get; set; }
}
