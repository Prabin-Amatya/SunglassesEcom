using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Sunglass_ecom.Data;
using Sunglass_ecom.Interfaces;
using Sunglass_ecom.Models;

namespace Sunglass_ecom.Repositoriess
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly EcommerceDbContext _dbcontext;
        public ServiceRepository(EcommerceDbContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public Task<List<Models.Product>> GetAllAsync(Productdto pdto)
        {
            return _dbcontext.Product.ToListAsync();
        }

        public async Task<List<Product>> SearchAsync(Productdto pdto)
        {
            var query = _dbcontext.Product.AsQueryable();
            if (!string.IsNullOrEmpty(pdto.ProductName))
            {
                query = query.Where(p => p.ProductName.Contains(pdto.ProductName));
            }

            if (!string.IsNullOrEmpty(pdto.CategoryName))
            {
                query = query.Where(p => p.Category != null && p.Category.Name.Contains(pdto.CategoryName));
            }
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Productdto>> GetAllProductDtosAsync()
        {
            var productDtos = await _dbcontext.Product
                .Include(p => p.Category)
                .Select(p => new Productdto
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    CategoryName = p.Category!=null ? p.Category.Name : null,
                    UnitPrice = p.UnitPrice
                })
                .ToListAsync();

            return productDtos;
        }

    }
}
