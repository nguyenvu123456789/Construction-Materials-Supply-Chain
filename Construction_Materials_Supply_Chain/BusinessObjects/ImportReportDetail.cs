namespace Domain
{
    public partial class ImportReportDetail
    {
        public int ImportReportDetailId { get; set; }
        public int ImportReportId { get; set; }
        public int MaterialId { get; set; }

        public int TotalQuantity { get; set; }
        public int GoodQuantity { get; set; }
        public int DamagedQuantity { get; set; }
        public string? Comment { get; set; }

        // 🔹 Navigation
        public virtual ImportReport ImportReport { get; set; } = null!;
        public virtual Material Material { get; set; } = null!;
    }
}
