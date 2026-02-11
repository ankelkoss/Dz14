using InternetShop.Dtos.OrderLine;
using InternetShop.Models;

namespace InternetShop.Dtos.Order
{
    public record OrderDto(int Id, int CustomerId, string? CustomerEmail, DateTime CreatedAtUtc, OrderStatus Status, List<OrderLineDto> Lines);
}
