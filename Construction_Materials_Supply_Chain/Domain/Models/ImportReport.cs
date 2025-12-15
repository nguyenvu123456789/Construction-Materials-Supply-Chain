namespace Domain.Models
{
    public partial class ImportReport
    {
        public int ImportReportId { get; set; }
        public int? ImportId { get; set; }
        public string ImportReportCode { get; set; } = string.Empty;
        public int? InvoiceId { get; set; }
        public int CreatedBy { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public virtual Import Import { get; set; } = null!;
        public virtual Invoice? Invoice { get; set; }
        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual ICollection<ImportReportDetail> ImportReportDetails { get; set; } = new List<ImportReportDetail>();
    }
}
