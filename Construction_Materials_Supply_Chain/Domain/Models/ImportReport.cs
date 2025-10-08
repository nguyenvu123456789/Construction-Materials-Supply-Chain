namespace Domain.Models
{
    public partial class ImportReport
    {
        public int ImportReportId { get; set; }
        public int? ImportId { get; set; } 
        public int? InvoiceId { get; set; }
        public int CreatedBy { get; set; }      
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending"; // Pending / Approved / Rejected
        public string? RejectReason { get; set; }

        public string? Notes { get; set; } // ghi chú thêm
        public DateTime? ReviewedAt { get; set; } // ngày duyệt
        public int? ReviewedBy { get; set; } // quản lý duyệt

        // 🔹 Navigation
        public virtual Import Import { get; set; } = null!;
        public virtual Invoice? Invoice { get; set; }
        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual User? ReviewedByNavigation { get; set; }
        public virtual ICollection<ImportReportDetail> ImportReportDetails { get; set; } = new List<ImportReportDetail>();
    }
}
