namespace Application.DTOs
{
    public class ImportRequestDto
    {
        public string InvoiceCode { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public int CreatedBy { get; set; }
    }
}
