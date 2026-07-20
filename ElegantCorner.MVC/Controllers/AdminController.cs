using ElegantCorner.Application.DTOs.Category;
using ElegantCorner.Application.DTOs.Product;
using ElegantCorner.Application.Interfaces;
using ElegantCorner.Application.Interfaces.Admin;
using ElegantCorner.Application.Services;
using ElegantCorner.MVC.Interfaces;
using ElegantCorner.MVC.Models.ViewModels;
using ElegantCorner.MVC.Models.ViewModels.Admin;
using ElegantCorner.MVC.Models.ViewModels.Product;
using ElegantCorner.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ElegantCorner.MVC.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _environment;
        private readonly IImageService _imageService;
        private readonly IProductService _productService;
        private readonly IProductAdminService _productAdminService;
        private readonly ICategoryAdminService _categoryAdminService;

        public AdminController(
            ICategoryService categoryService,
            IWebHostEnvironment environment,
            IImageService imageService,
            IProductService productService,
            IProductAdminService productAdminService,
            ICategoryAdminService categoryAdminService)
        {
            _categoryService = categoryService;
            _environment = environment;
            _imageService = imageService;
            _productService = productService;
            _productAdminService = productAdminService;
            _categoryAdminService = categoryAdminService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = new DashboardViewModel
            {
                CategoriesCount = (await _categoryService.GetAllAsync()).Count(),

                ProductsCount = (await _productService.GetAllAsync()).Count(),

                OrdersCount = 0,

                UsersCount = 0
            };

            return View(model);
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryService.GetAllAsync();

            return View(categories);
        }

        // GET
        public IActionResult CreateCategory()
        {
            return View();
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var imagePath = await _imageService.UploadAsync(model.Image!, "categories");

            var dto = new CreateCategoryDto
            {
                Name = model.Name,
                Description = model.Description,
                Icon = model.Icon,
                ImageUrl = imagePath
            };

            await _categoryService.CreateAsync(dto);

            return RedirectToAction(nameof(Categories));
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> EditCategory(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            var vm = new EditCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Icon = category.Icon,
                ExistingImageUrl = category.ImageUrl
            };

            return View(vm);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> EditCategory(EditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var imageUrl = model.ExistingImageUrl;

            if (model.Image != null)
                imageUrl = await _imageService.UploadAsync(model.Image, "categories");

            var dto = new CreateCategoryDto
            {
                Name = model.Name,
                Description = model.Description,
                Icon = model.Icon,
                ImageUrl = imageUrl
            };

            var updated = await _categoryService.UpdateAsync(model.Id, dto);

            if (!updated)
                return NotFound();

            return RedirectToAction(nameof(Categories));
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await _categoryService.DeleteAsync(id);

            return RedirectToAction(nameof(Categories));
        }

        public async Task<IActionResult> Products()
        {
            var products = await _productAdminService.GetAllAsync();

            var model = products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryName = p.CategoryName ?? "",
                Rating = p.Rating,
                ReviewsCount = p.ReviewsCount,
                IsAvailable = p.IsAvailable
            });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var vm = new ProductFormViewModel
            {
                Categories = (await _categoryAdminService.GetAllAsync())
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    })
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = (await _categoryAdminService.GetAllAsync())
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    });

                return View(vm);
            }

            string? imageUrl = null;

            if (vm.Image != null)
                imageUrl = await _imageService.UploadAsync(vm.Image, "products");

            var dto = new CreateProductDto
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                CategoryId = vm.CategoryId,
                ImageUrl = imageUrl,
                IsAvailable = vm.IsAvailable
            };

            await _productAdminService.CreateAsync(dto);

            return RedirectToAction(nameof(Products));
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(Guid id)
        {
            var product = await _productAdminService.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            var vm = new ProductFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,

                Categories = (await _categoryAdminService.GetAllAsync())
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    })
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = (await _categoryAdminService.GetAllAsync())
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    });

                return View(vm);
            }

            string? imageUrl = vm.ImageUrl;

            if (vm.Image != null)
            {
                imageUrl = await _imageService.UploadAsync(vm.Image, "products");
            }

            var dto = new UpdateProductDto
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                CategoryId = vm.CategoryId,
                ImageUrl = imageUrl,
                IsAvailable = vm.IsAvailable
            };

            await _productAdminService.UpdateAsync(dto);

            return RedirectToAction(nameof(Products));
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            await _productAdminService.DeleteAsync(id);

            return RedirectToAction(nameof(Products));
        }

        public IActionResult Orders()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }
    }

}