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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Category>> CreateCategory([FromForm]Category prod)
        {
            if (prod.CategoryImage != null)
            {
                string fileDirectory = $"wwwroot/CategoryImage";

                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                string uniqueFileName = Guid.NewGuid() + "_" + prod.CategoryImage.FileName;
                string filePath = Path.Combine(Path.GetFullPath($"wwwroot/CategoryImage"), uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await prod.CategoryImage.CopyToAsync(fileStream);
                    prod.Imageurl = $"CategoryImage/" + uniqueFileName;

                }

            }

            if (prod == null)
                {
                    return BadRequest("Category data is missing or invalid.");
                }
                await _dbContext.Category.AddAsync(prod);
                await _dbContext.SaveChangesAsync();
                return Ok(prod);

        }
        [HttpPut("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Category>> UpdateCategory(int Id, [FromForm] Category updatedCategory)
        {
            // Check if the incoming category ID matches the one in the URL
            if (Id != updatedCategory.Id)
            {
                return BadRequest("Category ID mismatch.");
            }

            // Find the existing category in the database
            var existingCategory = await _dbContext.Category.FindAsync(Id);
            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }

            // Update the fields with the values from the incoming category
            existingCategory.Name = updatedCategory.Name;
            existingCategory.CategoryDescription = updatedCategory.CategoryDescription;
            existingCategory.IsActive = updatedCategory.IsActive;

            // Handle image upload if a new image is provided
            if (updatedCategory.CategoryImage != null)
            {
                string fileDirectory = Path.Combine("wwwroot", "CategoryImage");

                // Create directory if it doesn't exist
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                string uniqueFileName = Guid.NewGuid() + "_" + updatedCategory.CategoryImage.FileName;
                string filePath = Path.Combine(fileDirectory, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await updatedCategory.CategoryImage.CopyToAsync(fileStream);
                    existingCategory.Imageurl = $"CategoryImage/{uniqueFileName}"; // Update the image URL
                }
            }

            // Save the changes to the database
            _dbContext.Entry(existingCategory).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return Ok(existingCategory); // Return the updated category
        }


        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
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
