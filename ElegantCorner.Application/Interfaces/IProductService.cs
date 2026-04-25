using ElegantCorner.Application.DTOs.Product;

namespace ElegantCorner.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(Guid categoryId);
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<bool> UpdateAsync(Guid id, CreateProductDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}