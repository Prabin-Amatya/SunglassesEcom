using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Recommendaton_Modal;
using Sunglass_ecom.Data;
using Sunglass_ecom.Models;

namespace Sunglass_ecom.Utils
{
    public class Vectors
    {
        public List<Vec> vectors;
        public BagOfWordsModel _model;
        public readonly IServiceScopeFactory _scope;

        public Vectors(BagOfWordsModel model, IServiceScopeFactory scope)
        {
            _model = model;
            _scope = scope;
            try
            {
                this.load();
                Console.WriteLine("Old Vectors Found");
            }
            catch (Exception ex)
            {
                this.createVectors();
                this.save();
                Console.WriteLine("New Vectors Created");
            }

        }

/*        public async Task initializeAsync(IServiceProvider service)
        {
            try
            {
                this.load();
                Console.WriteLine("Old Vectors Found");
            }
            catch (Exception ex)
            {
                contextOptions = service.GetRequiredService<DbContextOptions<EcommerceDbContext>>();
                _model = service.GetRequiredService<BagOfWordsModel>();
                this.createVectors();
                this.save();
                Console.WriteLine("New Vectors Created");
            }       
        }
*/
        public void createVectors()
        {
            using (var scope = _scope.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();
                var all_products = _context.Set<Product>();
                foreach (Product product in all_products)
                {
                    if (product.Description != null)
                    {
                        var token = _model.Tokenizer(product.Description, " ");
                        var vector = _model.Vectorizer(token);
                        Vec vec = new Vec();
                        vec.product = product;
                        vec.vector = vector;
                        this.vectors.Add(vec);
                    }
                }
            }
            this.save();
        }

        public void addVector(Product product)
        {

            if (product.Description != null)
            {
                var token = _model.Tokenizer(product.Description, " ");
                var vector = _model.Vectorizer(token);
                Vec vec = new Vec();
                vec.product = product;
                vec.vector = vector;
                this.vectors.Add(vec);
                this.save();
            }
        }

        public void deleteVector(int id)
        {
            var vectorToRemove = this.vectors.Find(x => x.product.Id == id);
            this.vectors.Remove(vectorToRemove);
            this.save();
        }

        public void save(string vocabfilepath = $"wwwroot/vectors.json")
        {
            try
            {
                using (StreamWriter fs = new StreamWriter(vocabfilepath))
                {
                    var set = JsonConvert.SerializeObject(this.vectors, Formatting.Indented);
                    fs.Write(set);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void load(string vocabfilepath = $"wwwroot/vectors.json", string dictfilepath = $"wwwroot/vectors.json")
        {

            this.vectors = new List<Vec>();
            try
            {
                using (StreamReader fs = new StreamReader(vocabfilepath))
                {
                    var vocabJson = fs.ReadToEnd();
                    if (vocabJson != null) { 
                        this.vectors = JsonConvert.DeserializeObject<List<Vec>>(vocabJson);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException();
                Console.WriteLine(ex.Message);
            }
        }




    }
}
