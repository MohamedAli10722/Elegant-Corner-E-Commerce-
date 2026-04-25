namespace ElegantCorner.Domain.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Quantity { get; set; } = 1;

        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}