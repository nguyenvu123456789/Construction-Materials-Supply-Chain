namespace Domain.Models
{
    public partial class Import
    {
        public int ImportId { get; set; }
        public string ImportCode { get; set; } = string.Empty;
        public DateTime ImportDate { get; set; } = DateTime.Now;
        public int WarehouseId { get; set; }
        public string Status { get; set; } = "Pending";  // Pending, Success, Cancelled
        public int CreatedBy { get; set; } // người tạo đơn
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual Warehouse Warehouse { get; set; } = null!;
        public virtual User ImportedByNavigation { get; set; } = null!;
        public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();
        public virtual ICollection<ImportReport> ImportReports { get; set; } = new List<ImportReport>();

    }
}
