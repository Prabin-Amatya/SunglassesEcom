using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sunglass_ecom.Models;
using Sunglass_ecom.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Sunglass_ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CategoryController : ControllerBase
    {
        private readonly EcommerceDbContext _dbContext;
        public CategoryController(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategory()
        {
            var data = await _dbContext.Category.ToListAsync();
            
            return Ok(data);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<List<Category>>> GetCategoryById(int Id)
        {
            var Category = await _dbContext.Category.FindAsync(Id);
            if (Category == null)
            {
                return NotFound("Id Not Found");
            }
            return Ok(Category);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Category>> CreateCategory([FromBody]Category prod)
        {
            
                if (prod == null)
                {
                    return BadRequest("Category data is missing or invalid.");
                }
                await _dbContext.Category.AddAsync(prod);
                await _dbContext.SaveChangesAsync();
                return Ok(prod);

        }
        [HttpPut("{Id}")]
        public async Task<ActionResult<Category>> UpdateCategory(int Id,Category Name)
        {
            if (Id != Name.Id)
            {
                return BadRequest();
            }
            _dbContext.Entry(Name).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return Ok(Name);
            
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int Id)
        {
            var prod = await _dbContext.Category.FindAsync(Id);
            if (prod == null)
            {
                return NotFound();
            }
           
             _dbContext.Category.Remove(prod);
            await _dbContext.SaveChangesAsync();

            return Ok();

            
        }
    }
}
