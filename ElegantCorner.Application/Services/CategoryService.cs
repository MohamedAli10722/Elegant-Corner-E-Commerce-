using ElegantCorner.Application.DTOs.Category;
using ElegantCorner.Application.Interfaces;
using ElegantCorner.Domain.Entities;
using ElegantCorner.Domain.Interfaces;

namespace ElegantCorner.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _uow.Categories.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                Icon = c.Icon
            });
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            var c = await _uow.Categories.GetByIdAsync(id);
            if (c == null) return null;
            return new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                Icon = c.Icon
            };
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Icon = dto.Icon
            };
            await _uow.Categories.AddAsync(category);
            await _uow.SaveChangesAsync();
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                Icon = category.Icon
            };
        }

        public async Task<bool> UpdateAsync(Guid id, CreateCategoryDto dto)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null) return false;
            category.Name = dto.Name;
            category.Description = dto.Description;
            category.ImageUrl = dto.ImageUrl;
            category.Icon = dto.Icon;
            _uow.Categories.Update(category);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null) return false;
            _uow.Categories.Delete(category);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}