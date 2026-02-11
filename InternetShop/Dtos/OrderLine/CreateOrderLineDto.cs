using System.ComponentModel.DataAnnotations;

namespace InternetShop.Dtos.OrderLine
{
    public class CreateOrderLineDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, 999999)]
        public int Quantity { get; set; }
    }
}
