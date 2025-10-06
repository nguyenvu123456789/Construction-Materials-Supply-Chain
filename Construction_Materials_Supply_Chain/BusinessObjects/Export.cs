namespace BusinessObjects
{
    public partial class Export
    {
        public int ExportId { get; set; }

        public DateTime ExportDate { get; set; }

        public int WarehouseId { get; set; }

        public string Status { get; set; } = "Pending";  // Pending, Success, Cancelled

        public int CreatedBy { get; set; } // người xuất (UserId)

        public string? Notes { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual Warehouse Warehouse { get; set; } = null!;
        public virtual User ExportedByNavigation { get; set; } = null!;
        public virtual ICollection<ExportDetail> ExportDetails { get; set; } = new List<ExportDetail>();
        public virtual ICollection<ExportReport> ExportReports { get; set; } = new List<ExportReport>();

    }
}
