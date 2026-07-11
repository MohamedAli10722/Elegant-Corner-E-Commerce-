namespace ElegantCorner.MVC.Models.ViewModels.Product
{
    public class ProductListViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public double Rating { get; set; }

        public int ReviewsCount { get; set; }
    }
}