namespace BusinessObjects;

public class MaterialCheck
{
    public int CheckId { get; set; }

    public int MaterialId { get; set; }

    public DateTime CheckDate { get; set; }

    public string? Result { get; set; }

    public string? Notes { get; set; }

    public virtual Material Material { get; set; } = null!;
}
