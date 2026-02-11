using System.ComponentModel.DataAnnotations;

namespace InternetShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Range(0, 999999999)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        // FK
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
