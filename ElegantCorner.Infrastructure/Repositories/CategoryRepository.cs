using ElegantCorner.Domain.Entities;
using ElegantCorner.Domain.Interfaces;
using ElegantCorner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElegantCorner.Infrastructure.Repositories;

public class CategoryRepository
    : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetAllWithProductsAsync()
    {
        return await _context.Categories
            .Include(c => c.Products)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdWithProductsAsync(Guid id)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}