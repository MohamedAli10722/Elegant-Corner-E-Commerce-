using ElegantCorner.Application.DTOs.Category;
using ElegantCorner.Application.DTOs.Product;

namespace ElegantCorner.MVC.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<CategoryDto> Categories { get; set; } = new();
        public List<ProductDto>  Products   { get; set; } = new();
    }
}
