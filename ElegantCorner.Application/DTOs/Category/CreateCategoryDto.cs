namespace ElegantCorner.Application.DTOs.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Icon { get; set; }
    }
}