using ElegantCorner.Application.DTOs.Product;

namespace ElegantCorner.Application.Interfaces.Admin;

public interface IProductAdminService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();

    Task<ProductDto?> GetByIdAsync(Guid id);

    Task CreateAsync(CreateProductDto dto);

    Task UpdateAsync(UpdateProductDto dto);

    Task DeleteAsync(Guid id);
}