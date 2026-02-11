namespace InternetShop.Models
{
    public enum OrderStatus
    {
        Draft = 0,
        Created = 1,
        Paid = 2,
        Shipped = 3,
        Completed = 4,
        Cancelled = 5
    }

    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Created;

        // FK
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        // Навигация
        public List<OrderLine> Lines { get; set; } = new();
    }
}
