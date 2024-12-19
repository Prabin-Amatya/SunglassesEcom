using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sunglass_ecom.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity.Data;
using System;
using Sunglass_ecom.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;


using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;

namespace Sunglass_ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly EcommerceDbContext _dbContext;
        private readonly IConfiguration _configuration;


        // Constructor to inject AppDbContext
        public RegistrationController(EcommerceDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("registration")]
        public async Task<IActionResult> Registration(User registration)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var hashedPassword = BC.HashPassword(registration.Password, workFactor: 10);
                    registration.Password = hashedPassword; 
                    Cart cart = new Cart();
                    await _dbContext.Set<Cart>().AddAsync(cart);
                    await _dbContext.SaveChangesAsync();
                    registration.cartId = cart.Id;
                    await _dbContext.Set<User>().AddAsync(registration);
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                User user = await _dbContext.Set<User>().Where(p => p.Username == loginDto.Username).SingleOrDefaultAsync();
                if (user == null)
                {
                    return BadRequest("Username invalid");
                }
                else
                {
                    if (BC.Verify(loginDto.Password, user.Password))
                    {
                        var claims = new List<Claim>{
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role)
                        };

                        var jwtToken = new JwtSecurityToken(
                                    claims: claims,
                                    notBefore: DateTime.UtcNow,
                                    expires: DateTime.UtcNow.AddDays(0.1),
                                    signingCredentials: new SigningCredentials(
                                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApplicationSettings:JWT_Secret"])
                                ), SecurityAlgorithms.HmacSha256Signature));
                        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                        var response = new
                        {
                            userName = user.Username,
                            token = token
                        };
                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest("password invalid");
                    }
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }



    }


}

