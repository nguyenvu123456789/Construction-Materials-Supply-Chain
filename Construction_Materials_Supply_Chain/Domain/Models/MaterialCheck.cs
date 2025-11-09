namespace Domain.Models;

public class MaterialCheck
{
    public int CheckId { get; set; }
    public int MaterialId { get; set; }
    public int UserId { get; set; }
    public DateTime CheckDate { get; set; }
    public string? Result { get; set; }
    public int QuantityChecked { get; set; }
    public string? Notes { get; set; }
    public virtual Material Material { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
