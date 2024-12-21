using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sunglass_ecom.Models;
using Sunglass_ecom.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Recommendaton_Modal;

namespace Sunglass_ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductController : ControllerBase
    {
        private readonly EcommerceDbContext _dbContext;
        private readonly BagOfWordsModel _modal;
        public ProductController(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
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
        [Authorize]
        public async Task<ActionResult<Product>> CreateProduct([FromBody]Product prod)
        {
            
                if (prod == null)
                {
                    return BadRequest("Product data is missing or invalid.");
                }
                await _dbContext.Product.AddAsync(prod);
                await _dbContext.SaveChangesAsync();
                return Ok(prod);

        }
        [HttpPut("{Id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int Id,Product Name)
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
    }
}
