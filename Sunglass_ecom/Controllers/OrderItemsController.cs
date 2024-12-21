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
        [HttpPost]
        public async Task<IActionResult> AddOrderItem(OrderItems orderItem)
        {

            if (orderItem == null)
            {
                return BadRequest("orderItemuct data is missing or invalid.");
            }
            await _dbContext.OrderItems.AddAsync(orderItem);
            await _dbContext.SaveChangesAsync();


            return Ok("Item added successfully");
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<OrderItems>> UpdateProduct(int Id, OrderItems Name)
        {
            if (Id != Name.Id)
            {
                return BadRequest();
            }
            _dbContext.Entry(Name).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return Ok(Name);




        }
    }

}