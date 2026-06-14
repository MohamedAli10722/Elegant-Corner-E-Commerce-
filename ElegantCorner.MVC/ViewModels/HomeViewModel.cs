using ElegantCorner.Application.DTOs.Category;
using ElegantCorner.Application.DTOs.Product;

namespace ElegantCorner.MVC.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<ProductDto>? Products { get; set; }
        public required IEnumerable<CategoryDto> Categories { get; set; }
    }
}
