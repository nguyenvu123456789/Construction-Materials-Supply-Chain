namespace Domain.Models;

public partial class ShippingLog
{
    public int ShippingLogId { get; set; }

    public int? OrderId { get; set; }

    public int? TransportId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Transport? Transport { get; set; }
}
