namespace Domain.Models
{
    public class MaterialCheckDetail
    {
        public int DetailId { get; set; }
        public int CheckId { get; set; }
        public int MaterialId { get; set; }

        public decimal SystemQty { get; set; }
        public decimal ActualQty { get; set; }
        public string? Reason { get; set; }

        // Navigation
        public virtual Material Material { get; set; } = null!;
        public virtual MaterialCheck Check { get; set; } = null!;
    }
}
