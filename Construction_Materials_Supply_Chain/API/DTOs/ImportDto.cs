namespace API.DTOs
{
    public class ImportDto
    {
        public string InvoiceCode { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime ImportDate { get; set; }
        public string? Note { get; set; } // nullable
        public string? WarehouseName { get; set; } // nullable

        public List<ImportDetailDto> Details { get; set; } = new List<ImportDetailDto>();
    }

    public class ImportDetailDto
    {
        public string MaterialName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
