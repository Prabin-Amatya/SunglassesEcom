using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sunglass_ecom.Models;
using Sunglass_ecom.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Recommendaton_Modal;
using Sunglass_ecom.Interfaces;
using Microsoft.ML;

namespace Sunglass_ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductController : ControllerBase
    {
        private readonly EcommerceDbContext _dbContext;
        private readonly BagOfWordsModel _modal;
        private  readonly IServiceRepository _serviceRepository;
        public ProductController (EcommerceDbContext dbContext, IServiceRepository servicerepo)
        {
            _dbContext = dbContext;
            _serviceRepository = servicerepo;
            _modal = new BagOfWordsModel();
            _modal.load();
           

        }
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProduct()
        {
            var data = await _dbContext.Product.ToListAsync();
            
            return Ok(data);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<List<Product>>> GetProductById(int Id)
        {
            var product = await _dbContext.Product.FindAsync(Id);

            if (product == null)
            {
                return NotFound("Id Not Found");
            }

            var currTokens = _modal.Tokenizer(product.Description, " ");
            var currVecs = _modal.Vectorizer(currTokens);
            var products = await _dbContext.Product.ToListAsync();
            foreach(Product prod in products)
            {
                if(prod.Id != product.Id)
                {
                    var otherTokens = _modal.Tokenizer(prod.Description, " ");
                    var otherVecs = _modal.Vectorizer(otherTokens);
                    prod.recommendationScore = BagOfWordsModel.cosineSimilarity(currVecs, otherVecs);
                }
            }

            products = products.OrderByDescending(e => e.recommendationScore).ToList();
            product.recommendations = products.Where(p=>p.Id!=product.Id).Take(10).ToList();
            if (product == null)
            {
                return NotFound("Id Not Found");
            }
            return Ok(product);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> CreateProduct([FromForm]Product prod)
        {
                 if (prod.ProductImage != null)
                    {
                        string fileDirectory = $"wwwroot/ProductImage";

                        if (!Directory.Exists(fileDirectory))
                        {
                            Directory.CreateDirectory(fileDirectory);
                        }
                        string uniqueFileName = Guid.NewGuid() + "_" + prod.ProductImage.FileName;
                        string filePath = Path.Combine(Path.GetFullPath($"wwwroot/ProductImage"), uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await prod.ProductImage.CopyToAsync(fileStream);
                            prod.Imageurl = $"ProductImage/" + uniqueFileName;

                        }

                    }

                if (prod == null)
                {
                    return BadRequest("Product data is missing or invalid.");
                }
                await _dbContext.Product.AddAsync(prod);
                await _dbContext.SaveChangesAsync();
                return Ok(prod);

        }


        [HttpPut("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> UpdateProduct(int Id,Product updatedProduct)
        {
            if (Id != updatedProduct.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            var existingProduct = await _dbContext.Product.FindAsync(Id);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            // Update fields with the values from the incoming product
            existingProduct.ProductName = updatedProduct.ProductName;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Manufacturer = updatedProduct.Manufacturer;
            existingProduct.UnitPrice = updatedProduct.UnitPrice;
            existingProduct.Discount = updatedProduct.Discount;
            existingProduct.Quantity = updatedProduct.Quantity;
            existingProduct.Status = updatedProduct.Status;
            existingProduct.IsActive = updatedProduct.IsActive;
            existingProduct.Stock = updatedProduct.Stock;
            existingProduct.CategoryId = updatedProduct.CategoryId;

            // Handle image upload if a new image is provided
            if (updatedProduct.ProductImage != null)
            {
                string fileDirectory = Path.Combine("wwwroot", "ProductImage");

                // Create directory if it doesn't exist
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                string uniqueFileName = Guid.NewGuid() + "_" + updatedProduct.ProductImage.FileName;
                string filePath = Path.Combine(fileDirectory, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await updatedProduct.ProductImage.CopyToAsync(fileStream);
                    existingProduct.Imageurl = $"ProductImage/{uniqueFileName}"; // Update the image URL
                }
            }

            // Save the changes to the database
            _dbContext.Entry(existingProduct).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return Ok(existingProduct); // Return the updated product
        }

        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> DeleteProduct(int Id)
        {
            var prod = await _dbContext.Product.FindAsync(Id);
            if (prod == null)
            {
                return NotFound();
            }
           
             _dbContext.Product.Remove(prod);
            await _dbContext.SaveChangesAsync();

            return Ok();     
        }
        [HttpGet("Search")]
        public async Task<IActionResult> SearchProducts([FromQuery] Productdto pdto)
        {
            if (_serviceRepository == null)
            {
                return BadRequest("Repository is not initialized.");
            }

            var query = await _serviceRepository.SearchAsync(pdto);
             return Ok(query);
        }
        

    }
}
