using InternetShop.Data;
using InternetShop.Dtos.Customer;
using InternetShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ShopDbContext _db;
        public CustomersController(ShopDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<List<CustomerDto>>> GetAll()
        {
            var items = await _db.Customers
                .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
                .Select(c => new CustomerDto(c.Id, c.FirstName, c.LastName, c.Email, c.Phone, c.CreatedAtUtc))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDto>> GetById(int id)
        {
            var c = await _db.Customers.FindAsync(id);
            if (c is null) return NotFound();
            return Ok(new CustomerDto(c.Id, c.FirstName, c.LastName, c.Email, c.Phone, c.CreatedAtUtc));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto dto)
        {
            var exists = await _db.Customers.AnyAsync(x => x.Email == dto.Email);
            if (exists) return Conflict("Customer with this email already exists.");

            var entity = new Customer
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim(),
                Phone = dto.Phone
            };

            _db.Customers.Add(entity);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id },
                new CustomerDto(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.Phone, entity.CreatedAtUtc));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCustomerDto dto)
        {
            var entity = await _db.Customers.FindAsync(id);
            if (entity is null) return NotFound();

            // email uniqueness check (кроме текущего)
            var exists = await _db.Customers.AnyAsync(x => x.Email == dto.Email && x.Id != id);
            if (exists) return Conflict("Customer with this email already exists.");

            entity.FirstName = dto.FirstName.Trim();
            entity.LastName = dto.LastName.Trim();
            entity.Email = dto.Email.Trim();
            entity.Phone = dto.Phone;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Customers.FindAsync(id);
            if (entity is null) return NotFound();

            _db.Customers.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
