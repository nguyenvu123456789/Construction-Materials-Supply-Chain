namespace API.DTOs
{
    public class ShippingLogDto
    {
        public int ShippingLogId { get; set; }
        public int? OrderId { get; set; }
        public int? TransportId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
