using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace Recommendaton_Modal
{
    public class BagOfWordsModel
    {
        private HashSet<string> vocab;
        private Dictionary<string, int> vectors;
        private static int length;

        public BagOfWordsModel()
        {
        }

        public  BagOfWordsModel(List<String> corpus, string sep) {
            this.vocab = new HashSet<string>();
            this.vectors = new Dictionary<string, int>();
            foreach (String description in corpus)
            {
                foreach(string word in description.Split(sep))
                {
                    vocab.Add(word.ToLower());
                    this.vectors[word.ToLower()] = 0;
                }
            }
            length = this.vectors.Count;
        }

        public List<String> Tokenizer(String word,String sep)
        {
            return new List<String>( word.ToLower().Split(sep) );
        }

        public List<int> Vectorizer(List<String> tokenized_word)
        {
            Dictionary<string, int> vectors = new Dictionary<string, int>(this.vectors);
            foreach(String word in tokenized_word)
            {
                bool exists = this.vocab.Contains(word);
                if (exists){
                    vectors[word] = 1;
                }
            }
            return new List<int>(vectors.Values);
        }

        public static float cosineSimilarity(List<int> vector1, List<int> vector2)
        {
            if (vector1.Count != vector2.Count) throw new Exception("Vectors Must Have Same Length");
            float dotProduct = 0;
            float magVec1 = 0;
            float magVec2 = 0;
            for (int i = 0; i < length; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magVec1 += vector1[i] * vector1[i];
                magVec2 += vector2[i] * vector2[i];
            }
 
            magVec1 = (float)Math.Sqrt(magVec1);
            magVec2 = (float)Math.Sqrt(magVec2);

            if(magVec1 == 0 || magVec2 == 0){
                return 0;
            }

            float cosineSimilarity = dotProduct / (magVec1 * magVec2);
            return cosineSimilarity;

        }

        public void save(string vocabfilepath = $"wwwroot/vocabulary.json", string dictfilepath =$"wwwroot/dictionary.json")
        {
            try
            {
                using (StreamWriter fs = new StreamWriter(vocabfilepath))
                {
                    var set = JsonConvert.SerializeObject(this.vocab, Formatting.Indented);
                    fs.Write(set);
                }

                using (StreamWriter fs = new StreamWriter(dictfilepath))
                {
                    var dict = JsonConvert.SerializeObject(this.vectors, Formatting.Indented);
                    fs.Write(dict);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void load(string vocabfilepath = $"wwwroot/vocabulary.json", string dictfilepath = $"wwwroot/dictionary.json")
        {

            this.vocab = new HashSet<string>();
            this.vectors = new Dictionary<string, int>();
            try
            {
                using (StreamReader fs = new StreamReader(vocabfilepath))
                {
                    var vocabJson = fs.ReadToEnd();
                    this.vocab = JsonConvert.DeserializeObject<HashSet<string>>(vocabJson);
                }

                using (StreamReader fs = new StreamReader(dictfilepath))
                {
                    var vocabJson = fs.ReadToEnd();
                    this.vectors = JsonConvert.DeserializeObject<Dictionary<string, int>>(vocabJson);
                }

                length = this.vocab.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
