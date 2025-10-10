namespace Application.DTOs
{
    public class StockCheckSummaryDto
    {
        public int TotalSkuNeedCheck { get; set; }
        public double AccuracyPercent { get; set; }
        public decimal TotalValueDiff { get; set; }
        public int ActiveCheckCount { get; set; }
    }

    public class StockCheckListItemDto
    {
        public string Code { get; set; } = "";
        public string Warehouse { get; set; } = "";
        public string InCharge { get; set; } = "";
        public string Status { get; set; } = "";
        public int SkuCount { get; set; }
        public decimal DiffQty { get; set; }
        public decimal DiffValue { get; set; }
        public DateTime CheckedAt { get; set; }
    }

    public class SkuDiffDto
    {
        public string Sku { get; set; } = "";
        public string MaterialName { get; set; } = "";
        public string Location { get; set; } = "";
        public decimal SystemQty { get; set; }
        public decimal ActualQty { get; set; }
        public decimal DiffQty { get; set; }
        public string Reason { get; set; } = "—";
        public string ActionText { get; set; } = "Điều chỉnh";
    }
}

