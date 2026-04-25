using ElegantCorner.Domain.Entities;
using ElegantCorner.Domain.Interfaces;
using ElegantCorner.Infrastructure.Data;

namespace ElegantCorner.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<Cart> Carts { get; private set; }
        public IGenericRepository<CartItem> CartItems { get; private set; }
        public IGenericRepository<Order> Orders { get; private set; }
        public IGenericRepository<OrderItem> OrderItems { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Categories = new GenericRepository<Category>(context);
            Products = new GenericRepository<Product>(context);
            Carts = new GenericRepository<Cart>(context);
            CartItems = new GenericRepository<CartItem>(context);
            Orders = new GenericRepository<Order>(context);
            OrderItems = new GenericRepository<OrderItem>(context);
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}