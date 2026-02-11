using InternetShop.Data;
using InternetShop.Dtos.Product;
using InternetShop.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ShopDbContext _db;

        public ProductsController(ShopDbContext db) => _db = db;

        // GET api/products?page=1&pageSize=20&categoryId=3&search=iphone
        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAll(
            int page = 1,
            int pageSize = 20,
            int? categoryId = null,
            string? search = null)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 200);

            var query = _db.Products.AsQueryable();

            if (categoryId is not null)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(s));
            }

            var items = await query
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto(
                    p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.IsActive,
                    p.CategoryId, p.Category!.Name
                ))
                .ToListAsync();

            return Ok(items);
        }

        // GET api/products/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var p = await _db.Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p is null) return NotFound();

            return Ok(new ProductDto(
                p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.IsActive,
                p.CategoryId, p.Category?.Name
            ));
        }

        // POST api/products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
        {
            var categoryExists = await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                return BadRequest($"CategoryId={dto.CategoryId} not found.");

            var entity = new Product
            {
                Name = dto.Name.Trim(),
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                IsActive = dto.IsActive,
                CategoryId = dto.CategoryId
            };

            _db.Products.Add(entity);
            await _db.SaveChangesAsync();

            // Вернём созданный продукт
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, await BuildDto(entity.Id));
        }

        // PUT api/products/5  (полная замена полей)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var entity = await _db.Products.FindAsync(id);
            if (entity is null) return NotFound();

            var categoryExists = await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                return BadRequest($"CategoryId={dto.CategoryId} not found.");

            entity.Name = dto.Name.Trim();
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.StockQuantity = dto.StockQuantity;
            entity.IsActive = dto.IsActive;
            entity.CategoryId = dto.CategoryId;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // PATCH api/products/5  (частичное обновление)
        // Content-Type: application/json-patch+json
        [HttpPatch("{id:int}")]
        public async Task<ActionResult<ProductDto>> Patch(int id, JsonPatchDocument<UpdateProductDto> patchDoc)
        {
            var entity = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (entity is null) return NotFound();

            // Маппим Entity -> UpdateProductDto
            var dto = new UpdateProductDto
            {
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                StockQuantity = entity.StockQuantity,
                IsActive = entity.IsActive,
                CategoryId = entity.CategoryId
            };

            patchDoc.ApplyTo(dto, ModelState);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Валидация после патча
            if (!TryValidateModel(dto)) return ValidationProblem(ModelState);

            var categoryExists = await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                return BadRequest($"CategoryId={dto.CategoryId} not found.");

            // Обновляем сущность
            var tracked = await _db.Products.FirstAsync(p => p.Id == id);
            tracked.Name = dto.Name.Trim();
            tracked.Description = dto.Description;
            tracked.Price = dto.Price;
            tracked.StockQuantity = dto.StockQuantity;
            tracked.IsActive = dto.IsActive;
            tracked.CategoryId = dto.CategoryId;

            await _db.SaveChangesAsync();

            return Ok(await BuildDto(id));
        }

        // DELETE api/products/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Products.FindAsync(id);
            if (entity is null) return NotFound();

            _db.Products.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private async Task<ProductDto> BuildDto(int id)
        {
            var p = await _db.Products
                .Include(x => x.Category)
                .FirstAsync(x => x.Id == id);

            return new ProductDto(
                p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.IsActive,
                p.CategoryId, p.Category?.Name
            );
        }
    }
}
