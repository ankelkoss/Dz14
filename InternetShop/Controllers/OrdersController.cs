using InternetShop.Data;
using InternetShop.Dtos.Order;
using InternetShop.Dtos.OrderLine;
using InternetShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ShopDbContext _db;
        public OrdersController(ShopDbContext db) => _db = db;

        // GET api/orders
        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAll()
        {
            var orders = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Lines).ThenInclude(l => l.Product)
                .OrderByDescending(o => o.CreatedAtUtc)
                .Select(o => new OrderDto(
                    o.Id,
                    o.CustomerId,
                    o.Customer!.Email,
                    o.CreatedAtUtc,
                    o.Status,
                    o.Lines.Select(l => new OrderLineDto(l.Id, l.ProductId, l.Product!.Name, l.Quantity, l.UnitPrice)).ToList()
                ))
                .ToListAsync();

            return Ok(orders);
        }

        // GET api/orders/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var o = await _db.Orders
                .Include(x => x.Customer)
                .Include(x => x.Lines).ThenInclude(l => l.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (o is null) return NotFound();

            return Ok(new OrderDto(
                o.Id, o.CustomerId, o.Customer?.Email, o.CreatedAtUtc, o.Status,
                o.Lines.Select(l => new OrderLineDto(l.Id, l.ProductId, l.Product?.Name, l.Quantity, l.UnitPrice)).ToList()
            ));
        }

        // POST api/orders  (создать заказ + линии)
        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto)
        {
            var customerExists = await _db.Customers.AnyAsync(c => c.Id == dto.CustomerId);
            if (!customerExists) return BadRequest($"CustomerId={dto.CustomerId} not found.");

            if (dto.Lines.Count == 0) return BadRequest("Order must contain at least 1 line.");

            // Загружаем продукты одним запросом
            var productIds = dto.Lines.Select(l => l.ProductId).Distinct().ToList();
            var products = await _db.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // Проверка наличия всех продуктов
            var missing = productIds.Where(id => !products.ContainsKey(id)).ToList();
            if (missing.Count > 0) return BadRequest($"Products not found: {string.Join(",", missing)}");

            // Создаём заказ
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                Status = OrderStatus.Created,
                CreatedAtUtc = DateTime.UtcNow
            };

            foreach (var line in dto.Lines)
            {
                var product = products[line.ProductId];

                // (опционально) контроль склада
                if (product.StockQuantity < line.Quantity)
                    return BadRequest($"Not enough stock for ProductId={product.Id}");

                order.Lines.Add(new OrderLine
                {
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    UnitPrice = product.Price
                });

                // (опционально) списание со склада
                product.StockQuantity -= line.Quantity;
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, await BuildDto(order.Id));
        }

        // PUT api/orders/5/status (пример узкого REST-эндпоинта)
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatus status)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order is null) return NotFound();

            order.Status = status;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/orders/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order is null) return NotFound();

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private async Task<OrderDto> BuildDto(int id)
        {
            var o = await _db.Orders
                .Include(x => x.Customer)
                .Include(x => x.Lines).ThenInclude(l => l.Product)
                .FirstAsync(x => x.Id == id);

            return new OrderDto(
                o.Id, o.CustomerId, o.Customer?.Email, o.CreatedAtUtc, o.Status,
                o.Lines.Select(l => new OrderLineDto(l.Id, l.ProductId, l.Product?.Name, l.Quantity, l.UnitPrice)).ToList()
            );
        }
    }
}
