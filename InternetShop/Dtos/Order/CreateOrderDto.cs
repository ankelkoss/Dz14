using InternetShop.Dtos.OrderLine;
using System.ComponentModel.DataAnnotations;

namespace InternetShop.Dtos.Order
{
    public class CreateOrderDto
    {
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        public List<CreateOrderLineDto> Lines { get; set; } = new();
    }
}
