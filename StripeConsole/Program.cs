using System;
using Stripe;
using StripeTest;

namespace StripeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            StripeConfiguration.ApiKey = "sk_test_XhPRH7ck5ZZ3XQhCKhcDH2jO00yNkFjQfv";
            // Console.WriteLine(StripeConfiguration.ApiVersion);
            var helper = new StripeHelper();
            var endpoint = helper.CreateWebhookEndpoint();
            Console.WriteLine(endpoint);
        }
    }
}
