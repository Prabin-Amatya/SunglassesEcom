using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sunglass_ecom.Models;
using Sunglass_ecom.Services;
using System.Text;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using YourNamespace.Models;

namespace Sunglass_ecom.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly KhaltiService _khaltiService;
        private readonly IConfiguration _configuration;

        public PaymentsController(KhaltiService khaltiService, IConfiguration configuration)
        {
            _khaltiService = khaltiService;
            _configuration = configuration;
        }

        [HttpPost("initiate")]
 
        public async Task<IActionResult> InitiatePayment(KhaltiPaymentRequest request)
            {
                var url = "https://dev.khalti.com/api/v2/epayment/initiate/";

                request.purchase_order_id = Guid.NewGuid().ToString();
                var jsonPayload = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "key " + _configuration["Khalti:SecretKey"]);

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                    // Deserialize the response to get the payment_url
                    var khaltiResponse = JsonConvert.DeserializeObject<KhaltiPaymentResponse>(responseContent);
                    
                    // Redirect the user to the payment URL
                    return Ok(new { url = khaltiResponse.payment_url });
                }
                else
                {
                    // Log the error or return an error message
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return BadRequest($"Failed to initiate payment. Error: {errorContent}");
                }
           }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] KhaltiVerifyRequest request)
        {
            try
            {
                var response = await _khaltiService.VerifyPaymentAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

}

    

