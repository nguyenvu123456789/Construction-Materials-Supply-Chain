namespace Domain.Models
{
    public class MaterialCheckDetail
    {
        public int DetailId { get; set; }
        public int CheckId { get; set; }
        public int MaterialId { get; set; }

        public decimal SystemQty { get; set; }       // Số lượng trong hệ thống
        public decimal ActualQty { get; set; }       // Số lượng kiểm kê thực tế
        public string? Reason { get; set; }          // Lý do lệch (nếu có)

        // Navigation
        public virtual Material Material { get; set; } = null!;
        public virtual MaterialCheck Check { get; set; } = null!;
    }
}
