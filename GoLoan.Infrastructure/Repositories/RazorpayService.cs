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

            return payment["status"].ToString() == "captured";
        }


        public bool VerifyPaymentDetails(string paymentId,string orderId,decimal expectedAmount)
        {
            string keyId = configuration["Razorpay:KeyId"];
            string keySecret = configuration["Razorpay:KeySecret"];

            RazorpayClient client = new RazorpayClient(keyId, keySecret);
            Payment payment = client.Payment.Fetch(paymentId);

            return payment["order_id"].ToString() == orderId
                && Convert.ToInt64(payment["amount"]) == decimal.ToInt64(expectedAmount * 100)
                && payment["currency"].ToString() == "INR"
                && payment["status"].ToString() == "captured";
        }

    }
}