using Microsoft.AspNetCore.Identity;

namespace ElegantCorner.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;
        public Cart? Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}