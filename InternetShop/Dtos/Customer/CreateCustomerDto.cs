using System.ComponentModel.DataAnnotations;

namespace InternetShop.Dtos.Customer
{
    public class CreateCustomerDto
    {
        [Required, MaxLength(120)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(120)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Phone { get; set; }
    }
}
