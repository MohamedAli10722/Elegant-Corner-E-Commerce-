using ElegantCorner.Application.Interfaces;
using ElegantCorner.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ElegantCorner.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: /Home/Index or /
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            var products = await _productService.GetAllAsync();

            var vm = new HomeViewModel
            {
                Products = products,
                Categories = categories
            };

            return View(vm);
        }
    }
}