using System.ComponentModel.DataAnnotations;

namespace InternetShop.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(120)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Phone { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Навигация
        public List<Order> Orders { get; set; } = new();
    }
}
