namespace Domain.Models
{
    public partial class ExportReport
    {
        public int ExportReportId { get; set; }

        public int ExportId { get; set; } // Liên kết với phiếu xuất kho
        public string ExportReportCode { get; set; } = null!;
        public DateTime ReportDate { get; set; } = DateTime.Now;

        public int ReportedBy { get; set; } // Thủ kho báo cáo

        public string Status { get; set; } = "Pending"; // Pending / Approved / Rejected

        public string? Notes { get; set; } // Lý do hư hỏng

        public DateTime? DecidedAt { get; set; } // Ngày quản lý duyệt

        public int? DecidedBy { get; set; } // Quản lý duyệt

        public virtual Export Export { get; set; } = null!;
        public virtual User ReportedByNavigation { get; set; } = null!;
        public virtual User? DecidedByNavigation { get; set; }
        public virtual ICollection<ExportReportDetail> ExportReportDetails { get; set; } = new List<ExportReportDetail>();
    }
}
