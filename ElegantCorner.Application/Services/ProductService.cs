using ElegantCorner.Application.DTOs.Product;
using ElegantCorner.Application.Interfaces;
using ElegantCorner.Domain.Entities;
using ElegantCorner.Domain.Interfaces;

namespace ElegantCorner.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;

        public ProductService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        private static ProductDto MapToDto(Product p) => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            Rating = p.Rating,
            ReviewsCount = p.ReviewsCount,
            IsAvailable = p.IsAvailable,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name
        };

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _uow.Products.GetAllAsync();
            return products.Where(p => !p.IsDeleted).Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(Guid categoryId)
        {
            var products = await _uow.Products.GetAllAsync();
            return products.Where(p => !p.IsDeleted && p.CategoryId == categoryId).Select(MapToDto);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var p = await _uow.Products.GetByIdAsync(id);
            if (p == null || p.IsDeleted) return null;
            return MapToDto(p);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };
            await _uow.Products.AddAsync(product);
            await _uow.SaveChangesAsync();
            return MapToDto(product);
        }

        public async Task<bool> UpdateAsync(Guid id, CreateProductDto dto)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null || product.IsDeleted) return false;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;
            _uow.Products.Update(product);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null) return false;
            product.IsDeleted = true;
            _uow.Products.Update(product);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}