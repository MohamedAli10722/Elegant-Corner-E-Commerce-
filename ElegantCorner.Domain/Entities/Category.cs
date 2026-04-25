namespace ElegantCorner.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Icon { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}