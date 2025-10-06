namespace Domain.Models;

public partial class Transport
{
    public int TransportId { get; set; }

    public string? Vehicle { get; set; }

    public string? Driver { get; set; }

    public string? Porter { get; set; }

    public string? Route { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<ShippingLog> ShippingLogs { get; set; } = new List<ShippingLog>();
}
