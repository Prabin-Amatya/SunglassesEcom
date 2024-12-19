using System.ComponentModel.DataAnnotations.Schema;
namespace Sunglass_ecom.Models
{
    public class Productdto
    {
      
        
            public int Id { get; set; }
            public string ProductName { get; set; }
            public string Manufacturer { get; set; }
            public decimal UnitPrice { get; set; }
            public int Stock { get; set; }
            public string ImageUrl { get; set; }
             public decimal? Discount { get; set; }

             public int Quantity { get; set; }
    }
}
