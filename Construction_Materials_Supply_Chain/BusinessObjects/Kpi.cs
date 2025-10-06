namespace BusinessObjects;

public partial class Kpi
{
    public int Kpiid { get; set; }

    public string? Kpiname { get; set; }

    public string? Description { get; set; }

    public decimal? TargetValue { get; set; }

    public decimal? CurrentValue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
