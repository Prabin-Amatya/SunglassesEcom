using Sunglass_ecom.Data;

namespace Sunglass_ecom.Services
{
    public class StockService
    {
        private readonly EcommerceDbContext _dbcontext;

        public StockService(EcommerceDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> UpdateStock(int productId, int quantity, string type)
        {
            var product = await _dbcontext.Product.FindAsync(productId);
            if (product == null) return false;

            if (type == "Inbound") product.Quantity += quantity;
            else if (type == "Outbound") product.Quantity -= quantity;

            _dbcontext.Product.Update(product);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
    }
}
