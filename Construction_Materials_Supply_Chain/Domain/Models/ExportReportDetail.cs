namespace Domain.Models
{
    public partial class ExportReportDetail
    {
        public int ExportReportDetailId { get; set; }

        public int ExportReportId { get; set; }

        public int MaterialId { get; set; }

        public decimal QuantityDamaged { get; set; }

        public string Reason { get; set; } = null!;

        public bool? Keep { get; set; } = false;

        public virtual ExportReport ExportReport { get; set; } = null!;
        public virtual Material Material { get; set; } = null!;
    }
}
