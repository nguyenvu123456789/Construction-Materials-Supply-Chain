namespace Domain.Models
{
    public class MaterialCheck
    {
        public int CheckId { get; set; }

        public int WarehouseId { get; set; }       // Kiểm kê ở kho nào
        public int UserId { get; set; }            // Ai là người kiểm kê
        public DateTime CheckDate { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Pending";

        // Navigation
        public virtual Warehouse Warehouse { get; set; } = null!;
        public virtual User User { get; set; } = null!;

        public virtual ICollection<MaterialCheckDetail> Details { get; set; }
            = new List<MaterialCheckDetail>();
    }
}
