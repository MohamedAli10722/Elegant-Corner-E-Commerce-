using ElegantCorner.Domain.Entities;
using ElegantCorner.Domain.Interfaces;
using ElegantCorner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElegantCorner.Infrastructure.Repositories;

public class ProductRepository
    : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
    {
        return await _context.Products
            .Include(x => x.Category)
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdWithCategoryAsync(Guid id)
    {
        return await _context.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }
}