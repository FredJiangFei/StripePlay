using System;
using System.Collections.Generic;
using Stripe;

namespace StripeTest
{
    public class StripeHelper
    {
        public void CreateCustomer (){
            var options = new CustomerCreateOptions
            {
                Description = "My First Test Customer",
            };
            var service = new CustomerService();
            service.Create(options);
        }

        public void CreateAPaymentIntent(){
            var options = new PaymentIntentCreateOptions
            {
                Customer = "cus_LVSOSDDyOkR9Qe",
                Currency = "usd",
                Amount = 2000,
                PaymentMethodTypes = new List<string> { "card" },
                SetupFutureUsage = "on_session",
            };
            var service = new PaymentIntentService();
            service.Create(options);
        }

        public void Authentication (){
            Dictionary<string, string> Metadata = new Dictionary<string, string>();
            Metadata.Add("Product", "RubberDuck");
            Metadata.Add("Quantity", "10");
            var options = new ChargeCreateOptions
            {
                Amount = 100,
                Currency = "USD",
                Description = "Buying 10 rubber ducks",
                // Source = stripeToken,
                // ReceiptEmail = stripeEmail,
                Metadata = Metadata
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);
            Console.Write(charge);
        }
        
    }
}
