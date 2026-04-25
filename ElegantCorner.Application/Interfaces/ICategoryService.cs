using ElegantCorner.Application.DTOs.Category;

namespace ElegantCorner.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(Guid id);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<bool> UpdateAsync(Guid id, CreateCategoryDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}