using ElegantCorner.Application.Interfaces;
using ElegantCorner.MVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ElegantCorner.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService  _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService  = productService;
            _categoryService = categoryService;
        }

        // GET: /
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            var products   = await _productService.GetAllAsync();

            var vm = new HomeViewModel
            {
                Categories = categories.ToList(),
                Products   = products.ToList()
            };

            return View(vm);
        }

        // AJAX: GET /Home/Products?categoryId=...
        [HttpGet]
        public async Task<IActionResult> Products(Guid? categoryId)
        {
            var products = categoryId.HasValue
                ? await _productService.GetByCategoryAsync(categoryId.Value)
                : await _productService.GetAllAsync();

            return Json(products);
        }

        // Error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
