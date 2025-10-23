namespace Application.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
    public class CreateCategoryRequest
    {
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
