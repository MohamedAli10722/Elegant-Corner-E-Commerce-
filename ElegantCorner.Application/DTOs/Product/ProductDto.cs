namespace ElegantCorner.Application.DTOs.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public double Rating { get; set; }
        public int ReviewsCount { get; set; }
        public bool IsAvailable { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}