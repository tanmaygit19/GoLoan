using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System.Collections.Generic;

namespace GoLoan.Infrastructure.Repositories
{
    public class RazorpayService
    {
        private readonly IConfiguration configuration;

        public RazorpayService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateOrder(decimal amount, int requestId)
        {
            string keyId = configuration["Razorpay:KeyId"];
            string keySecret = configuration["Razorpay:KeySecret"];

            RazorpayClient client = new RazorpayClient(keyId, keySecret);

            var options = new Dictionary<string, object>
            {
                { "amount", decimal.ToInt64(amount * 100) },
                { "currency", "INR" },
                { "receipt", $"FC_{requestId}" }
            };

            Console.WriteLine($"Foreclosure Amount (Rupees): {amount}");
            Console.WriteLine($"Foreclosure Amount (Paise): {decimal.ToInt64(amount * 100)}");

            Order order = client.Order.Create(options);

            return order["id"].ToString();
        }


        public bool VerifyPayment(string orderId, string paymentId, string signature)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("razorpay_order_id", orderId);
            data.Add("razorpay_payment_id", paymentId);
            data.Add("razorpay_signature", signature);

            Utils.verifyPaymentSignature(data);

            return true;
        }

        public bool IsPaymentSuccessful(string paymentId)
        {
            string keyId = configuration["Razorpay:KeyId"];
            string keySecret = configuration["Razorpay:KeySecret"];

            RazorpayClient client = new RazorpayClient(keyId, keySecret);
            Payment payment = client.Payment.Fetch(paymentId);

            string status = payment["status"].ToString();

            Console.WriteLine("Payment ID: " + paymentId);
            Console.WriteLine("Payment Status: " + status);

            return status == "captured";
        }


        public bool VerifyPaymentDetails(string paymentId,string orderId,decimal expectedAmount)
        {
            string keyId = configuration["Razorpay:KeyId"];
            string keySecret = configuration["Razorpay:KeySecret"];

            RazorpayClient client = new RazorpayClient(keyId, keySecret);
            Payment payment = client.Payment.Fetch(paymentId);
            Console.WriteLine("Order ID: " + payment["order_id"]);
            Console.WriteLine("Amount: " + payment["amount"]);
            Console.WriteLine("Currency: " + payment["currency"]);
            Console.WriteLine("Status: " + payment["status"]);
            return payment["order_id"].ToString() == orderId
                && Convert.ToInt64(payment["amount"]) == decimal.ToInt64(expectedAmount * 100)
                && payment["currency"].ToString() == "INR"
                && payment["status"].ToString() == "captured";
        }

        public long GetOrderAmount(string orderId)
        {
            string keyId = configuration["Razorpay:KeyId"]!;
            string keySecret = configuration["Razorpay:KeySecret"]!;

            RazorpayClient client = new RazorpayClient(keyId, keySecret);

            Order order = client.Order.Fetch(orderId);

            return Convert.ToInt64(order["amount"]);
        }

    }
}