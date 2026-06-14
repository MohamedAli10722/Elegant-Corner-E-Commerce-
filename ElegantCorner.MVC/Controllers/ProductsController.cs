using ElegantCorner.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElegantCorner.MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: /Products or /Products?categoryId=xxx
        public async Task<IActionResult> Index(Guid? categoryId)
        {
            var categories = await _categoryService.GetAllAsync();
            var products = categoryId.HasValue
                ? await _productService.GetByCategoryAsync(categoryId.Value)
                : await _productService.GetAllAsync();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;

            if (categoryId.HasValue)
            {
                var category = categories.FirstOrDefault(c => c.Id == categoryId);
                ViewBag.PageTitle = $"{category?.Name} Products";
            }
            else
            {
                ViewBag.PageTitle = "All Products";
            }

            return View(products);
        }

        // GET: /Products/Details/id
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}