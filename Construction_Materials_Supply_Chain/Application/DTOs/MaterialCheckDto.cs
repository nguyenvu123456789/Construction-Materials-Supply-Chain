namespace Application.DTOs
{
    public class MaterialCheckDto
    {
        public int CheckId { get; set; }
        public int MaterialId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckDate { get; set; }
        public string? Result { get; set; }
        public int QuantityChecked { get; set; }
        public string? Notes { get; set; }
    }
}
