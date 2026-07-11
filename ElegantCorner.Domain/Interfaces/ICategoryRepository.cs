using ElegantCorner.Domain.Entities;

namespace ElegantCorner.Domain.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetAllWithProductsAsync();

    Task<Category?> GetByIdWithProductsAsync(Guid id);
}