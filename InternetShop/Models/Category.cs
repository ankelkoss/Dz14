using System.ComponentModel.DataAnnotations;

namespace InternetShop.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        // Навигация
        public List<Product> Products { get; set; } = new();
    }
}
