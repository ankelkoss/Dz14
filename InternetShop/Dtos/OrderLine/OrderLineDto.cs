namespace InternetShop.Dtos.OrderLine
{
    public record OrderLineDto(int Id, int ProductId, string? ProductName, int Quantity, decimal UnitPrice);
}
