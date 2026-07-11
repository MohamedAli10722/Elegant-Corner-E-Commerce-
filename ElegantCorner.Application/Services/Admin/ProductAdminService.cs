using ElegantCorner.Application.DTOs.Product;
using ElegantCorner.Application.Interfaces.Admin;
using ElegantCorner.Domain.Entities;
using ElegantCorner.Domain.Interfaces;

namespace ElegantCorner.Application.Services.Admin;

public class ProductAdminService : IProductAdminService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductAdminService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _unitOfWork.Products
            .GetAllWithIncludeAsync(x => x.Category);

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            Rating = p.Rating,
            ReviewsCount = p.ReviewsCount,
            IsAvailable = p.IsAvailable
        });
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Products
            .GetByIdWithIncludeAsync(id, x => x.Category);

        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            Rating = product.Rating,
            ReviewsCount = product.ReviewsCount,
            IsAvailable = product.IsAvailable
        };
    }

    public async Task CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId,
            IsAvailable = dto.IsAvailable
        };

        await _unitOfWork.Products.AddAsync(product);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateProductDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);

        if (product == null)
            return;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.ImageUrl = dto.ImageUrl;
        product.CategoryId = dto.CategoryId;
        product.IsAvailable = dto.IsAvailable;
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Products.Update(product);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
            return;

        product.IsDeleted = true;

        _unitOfWork.Products.Update(product);

        await _unitOfWork.SaveChangesAsync();
    }
}