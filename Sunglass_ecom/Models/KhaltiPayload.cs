namespace YourNamespace.Models
{
    public class KhaltiPaymentRequest
    {
        public string return_url { get; set; }           // URL to redirect after payment
        public string website_url { get; set; }          // Website URL
        public string amount { get; set; }               // Amount to be charged
        public string? purchase_order_id { get; set; }   // Unique ID for the purchase order
        public string purchase_order_name { get; set; }  // Name or description of the purchase
        public CustomerInfo customer_info { get; set; }  // Customer information
    }

    public class CustomerInfo
    {
        public string name { get; set; }                 // Customer's name
        public string email { get; set; }                // Customer's email
        public string phone { get; set; }                // Customer's phone number
    }
}
