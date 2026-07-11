using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ElegantCorner.MVC.Models.ViewModels.Product
{
    public class CreateProductViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(1, 1000000)]
        public decimal Price { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public bool IsAvailable { get; set; } = true;

        public IFormFile? Image { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
    }
}