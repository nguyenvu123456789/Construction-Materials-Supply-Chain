namespace Domain.Models
{
    public partial class ExportReportDetail
    {
        public int ExportReportDetailId { get; set; }

        public int ExportReportId { get; set; }

        public int MaterialId { get; set; }

        public decimal Quantity { get; set; }

        public string Reason { get; set; } = null!; // Lý do hư hỏng

        public bool Keep { get; set; } = false;

        public virtual ExportReport ExportReport { get; set; } = null!;
        public virtual Material Material { get; set; } = null!;
    }
}
