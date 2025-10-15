namespace Application.DTOs
{
    public class CreateInvoiceDto
    {
        public string InvoiceCode { get; set; } = null!;
        public string InvoiceType { get; set; } = null!;
        public int PartnerId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? DueDate { get; set; }
        public List<CreateInvoiceDetailDto> Details { get; set; } = new List<CreateInvoiceDetailDto>();
    }

    public class CreateInvoiceDetailDto
    {
        public int MaterialId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class CreateInvoiceFromOrderDto
    {
        public string OrderCode { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? DueDate { get; set; }

        public List<InvoiceUnitPriceDto> UnitPrices { get; set; } = new();
    }

    public class InvoiceUnitPriceDto
    {
        public int MaterialId { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class MaterialPriceDto
    {
        public int MaterialId { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
