namespace InternetShop.Dtos.Customer
{
    public record CustomerDto(int Id, string FirstName, string LastName, string Email, string? Phone, DateTime CreatedAtUtc);
}
