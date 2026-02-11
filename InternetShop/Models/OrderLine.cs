using System.ComponentModel.DataAnnotations;

namespace InternetShop.Models
{
    public class OrderLine
    {
        public int Id { get; set; }

        // FK
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Range(1, 999999)]
        public int Quantity { get; set; }

        [Range(0, 999999999)]
        public decimal UnitPrice { get; set; } // цена на момент заказа
    }
}
