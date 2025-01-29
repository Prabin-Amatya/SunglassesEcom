using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sunglass_ecom.Data;
using Sunglass_ecom.Models;

namespace Sunglass_ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly EcommerceDbContext _dbContext;

        public OrderItemsController(EcommerceDbContext context)
        {
            _dbContext = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItems>>> GetAllOrderItems()
        {
            var orderItems = await _dbContext.OrderItems.ToListAsync();
            return Ok(orderItems);
        }

        // GET: api/OrderItems/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItems>> GetOrderItemById(int id)
        {
            var orderItem = await _dbContext.OrderItems.FindAsync(id);

            if (orderItem == null)
            {
                return NotFound("Order item not found.");
            }

            return Ok(orderItem);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrderItem(OrderItems orderItem)
        {

            if (orderItem == null)
            {
                return BadRequest("orderItemuct data is missing or invalid.");
            }
            await _dbContext.OrderItems.AddAsync(orderItem);
            await _dbContext.SaveChangesAsync();


            return Ok(orderItem);
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<OrderItems>> UpdateProduct(int Id, OrderItems updatedItem)
        {
            if (Id != updatedItem.Id)
            {
                return BadRequest();
            }

            var existingItem = await _dbContext.OrderItems.FindAsync(Id);
            if (existingItem == null)
            {
                return NotFound();
            }

            // Update all properties
            existingItem.CartId = updatedItem.CartId;
            existingItem.ProductId = updatedItem.ProductId;
            existingItem.ProductName = updatedItem.ProductName;
            existingItem.CategoryName = updatedItem.CategoryName;
            existingItem.UnitPrice = updatedItem.UnitPrice;
            existingItem.Discount = updatedItem.Discount;
            existingItem.Quantity = updatedItem.Quantity;
            existingItem.TotalPrice = updatedItem.TotalPrice;
            existingItem.SubTotal = updatedItem.SubTotal;
            existingItem.status = updatedItem.status;
            existingItem.isActive = updatedItem.isActive;

            await _dbContext.SaveChangesAsync();
            return Ok(existingItem);
        }

    }

}