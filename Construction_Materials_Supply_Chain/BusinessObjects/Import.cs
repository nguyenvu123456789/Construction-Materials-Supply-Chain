namespace BusinessObjects
{
    public partial class Import
    {
        public int ImportId { get; set; }

        public DateTime ImportDate { get; set; }

        public int WarehouseId { get; set; }

        public string Status { get; set; } = "Pending";  // Pending, Success, Cancelled

        public int CreatedBy { get; set; } // người nhập (UserId)

        public string? Notes { get; set; }                                                   

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Quan hệ
        public virtual Warehouse Warehouse { get; set; } = null!;
        public virtual User ImportedByNavigation { get; set; } = null!;
        public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();
    }
}
