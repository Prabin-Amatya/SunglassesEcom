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
        [HttpGet("{UserId}")]
        public async Task<ActionResult<List<Cart>>> GetUserId(int UserId)
        {
            var Userid = await _dbContext.Cart.FindAsync(UserId);
            if (Userid == null)
            {
                return NotFound("Id not found");
            }
            return Ok(Userid);
        }

        [HttpPost("AddToCart")]
        public async Task<ActionResult<Cart>> AddToCart([FromBody] Cart cart)
        {

            if (cart == null)
            {
                return BadRequest("orderItemuct data is missing or invalid.");
            }
            await _dbContext.Cart.AddAsync(cart);
            await _dbContext.SaveChangesAsync();


            return Ok("Item added successfully");
        }

        [HttpDelete("RemoveFromCart/{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _dbContext.Cart.FindAsync(id);
            if (item == null)
                return NotFound("Item not found");

            _dbContext.Cart.Remove(item);
            await _dbContext.SaveChangesAsync();
            return Ok("Item removed");
        }

    }

    } 


