using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sunglass_ecom.Data;
using Sunglass_ecom.Models;


namespace Sunglass_ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CartController : ControllerBase
    {
        private readonly EcommerceDbContext _dbContext;

        public CartController(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<List<Cart>>> GetCart(int Id)
        {
            var data = await _dbContext.Cart.FindAsync(Id);
            if (data == null)
            {
                return NotFound("Id not found");
            }
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult<Cart>> AddToCart([FromBody] Cart cart)
        {

            if (cart == null)
            {
                return BadRequest("orderItem data is missing or invalid.");
            }
            await _dbContext.Cart.AddAsync(cart);
            await _dbContext.SaveChangesAsync();


            return Ok("Item added successfully");
        }

        [HttpPut("{cart}")]
        public async Task<IActionResult> UpdateCart(Cart cart)
        {
            Cart old_cart = await _dbContext.Cart.FirstOrDefaultAsync(p => p.Id == cart.Id);
            old_cart.IsActive = cart.IsActive;
            old_cart.TotalPrice = cart.TotalPrice;
            old_cart.Quantity = cart.Quantity;
            old_cart.UnitPrice = cart.UnitPrice;
            old_cart.OrderItems = cart.OrderItems;
            _dbContext.Update(old_cart);
            await _dbContext.SaveChangesAsync();
            return Ok("Item removed");
        }

    }

    } 


