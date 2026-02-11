using System.ComponentModel.DataAnnotations;

namespace InternetShop.Dtos.Product
{
    public class CreateProductDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Range(0, 999999999)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }

}
