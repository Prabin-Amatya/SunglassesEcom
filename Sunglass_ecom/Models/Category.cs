using System.ComponentModel.DataAnnotations.Schema;

namespace Sunglass_ecom.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string CategoryDescription {get; set; }
        public string? Imageurl { get; set; }
        [NotMapped]
        public IFormFile? CategoryImage { get; set; }

        public bool? IsActive { get; set; }

        public  ICollection<Product>? Product { get; set; }
    }
}
