using System.ComponentModel.DataAnnotations;

namespace InternetShop.Dtos.Product
{
    public record ProductDto(
        int Id,
        string Name,
        string? Description,
        decimal Price,
        int StockQuantity,
        bool IsActive,
        int CategoryId,
        string? CategoryName
    );
}
