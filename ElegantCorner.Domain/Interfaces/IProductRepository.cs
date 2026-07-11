using ElegantCorner.Domain.Entities;

namespace ElegantCorner.Domain.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithCategoryAsync();

    Task<Product?> GetByIdWithCategoryAsync(Guid id);
}