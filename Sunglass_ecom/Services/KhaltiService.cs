using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sunglass_ecom.Models;

namespace Sunglass_ecom.Services
{
    public class KhaltiService
    {
        private readonly KhaltiSettings _khaltiSettings;

        public KhaltiService(IOptions<KhaltiSettings> khaltiSettings)
        {
            _khaltiSettings = khaltiSettings.Value;
        }

        public async Task<KhaltiVerifyResponse> VerifyPaymentAsync(KhaltiVerifyRequest request)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Key {_khaltiSettings.SecretKey}");

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("token", request.Token),
            new KeyValuePair<string, string>("amount", request.Amount.ToString())
        });

            var response = await client.PostAsync(_khaltiSettings.VerifyUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Payment verification failed!");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<KhaltiVerifyResponse>(responseString);
        }
    }
}
