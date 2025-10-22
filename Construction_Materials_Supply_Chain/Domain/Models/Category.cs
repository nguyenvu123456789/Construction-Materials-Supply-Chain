namespace Domain.Models;

public partial class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string Status { get; set; } = "Active";
    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}