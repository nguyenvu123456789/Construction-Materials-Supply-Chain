﻿using System.Text.Json.Serialization;

namespace Domain.Models
{
    public partial class Material
    {
        public int MaterialId { get; set; }
        public string MaterialCode { get; set; } = null!;
        public string MaterialName { get; set; } = null!;
        public int CategoryId { get; set; }
        public int PartnerId { get; set; }
        public string Unit { get; set; } = null!;
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public virtual Category? Category { get; set; }

        [JsonIgnore]
        public virtual Partner? Partner { get; set; }

        [JsonIgnore]
        public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();

        [JsonIgnore]
        public virtual ICollection<ImportReportDetail> ImportReportDetails { get; set; } = new List<ImportReportDetail>();

        [JsonIgnore]
        public virtual ICollection<ExportDetail> ExportDetails { get; set; } = new List<ExportDetail>();

        [JsonIgnore]
        public virtual ICollection<ExportReportDetail> ExportReportDetails { get; set; } = new List<ExportReportDetail>();

        [JsonIgnore]
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        [JsonIgnore]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

        [JsonIgnore]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        [JsonIgnore]
        public virtual ICollection<MaterialCheck> MaterialChecks { get; set; } = new List<MaterialCheck>();
    }
}
