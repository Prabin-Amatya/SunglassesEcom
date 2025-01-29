using System;

namespace YourNamespace.Models
{
    public class KhaltiPaymentResponse
    {
        public string pidx { get; set; }
        public string payment_url { get; set; }
        public DateTime expires_at { get; set; }
        public int expires_in { get; set; }
    }
}