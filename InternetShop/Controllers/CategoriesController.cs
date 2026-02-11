using InternetShop.Data;
using InternetShop.Dtos.Category;
using InternetShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ShopDbContext _db;

        public CategoriesController(ShopDbContext db) => _db = db;

        // GET api/categories
        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetAll()
        {
            var items = await _db.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto(c.Id, c.Name, c.Description))
                .ToListAsync();

            return Ok(items);
        }

        // GET api/categories/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var entity = await _db.Categories.FindAsync(id);
            if (entity is null) return NotFound();

            return Ok(new CategoryDto(entity.Id, entity.Name, entity.Description));
        }

        // POST api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto)
        {
            var entity = new Category
            {
                Name = dto.Name.Trim(),
                Description = dto.Description
            };

            _db.Categories.Add(entity);
            await _db.SaveChangesAsync();

            var result = new CategoryDto(entity.Id, entity.Name, entity.Description);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
        }

        // PUT api/categories/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
        {
            var entity = await _db.Categories.FindAsync(id);
            if (entity is null) return NotFound();

            entity.Name = dto.Name.Trim();
            entity.Description = dto.Description;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/categories/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Categories.FindAsync(id);
            if (entity is null) return NotFound();

            _db.Categories.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
