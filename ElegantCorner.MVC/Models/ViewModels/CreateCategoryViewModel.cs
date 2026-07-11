using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ElegantCorner.MVC.Models.ViewModels
{
    public class CreateCategoryViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public string? Icon { get; set; }

        public IFormFile? Image { get; set; }
    }
}