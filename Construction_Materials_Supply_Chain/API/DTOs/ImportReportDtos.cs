namespace API.DTOs
{
    // Thủ kho tạo báo cáo
    public class ImportReportCreateDto
    {
        public int ImportId { get; set; }
        public int CreatedBy { get; set; } // UserId của thủ kho
        public string? Notes { get; set; }

        public List<ImportReportDetailCreateDto> Details { get; set; } = new();
    }

    public class ImportReportDetailCreateDto
    {
        public int MaterialId { get; set; }
        public int TotalQuantity { get; set; } // tổng nhập theo phiếu
        public int GoodQuantity { get; set; }  // hàng tốt
        public int DamagedQuantity { get; set; } // hàng hư
        public string? Comment { get; set; }
    }

    // Quản lý duyệt
    public class ImportReportReviewDto
    {
        public int ReportId { get; set; }
        public int ReviewedBy { get; set; }
        public bool Approved { get; set; }
        public string? RejectReason { get; set; }
    }
}
