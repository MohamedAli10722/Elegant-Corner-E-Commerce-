namespace ElegantCorner.Application.DTOs.Product
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public Guid CategoryId { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}