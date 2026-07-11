using ElegantCorner.Application.DTOs.Category;

namespace ElegantCorner.Application.Interfaces.Admin;

public interface ICategoryAdminService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();

    Task<CategoryDto?> GetByIdAsync(Guid id);

    Task CreateAsync(CreateCategoryDto dto);

    Task UpdateAsync(UpdateCategoryDto dto);

    Task DeleteAsync(Guid id);
}