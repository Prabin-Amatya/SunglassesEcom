
using Sunglass_ecom.Models;

namespace Sunglass_ecom.Interfaces
{
    public interface IServiceRepository
    {
        Task<List<Product>> GetAllAsync(Productdto pdto);
        Task<List<Product>> SearchAsync(Productdto pdto);
        Task<IEnumerable<Productdto>> GetAllProductDtosAsync();
    }
}
