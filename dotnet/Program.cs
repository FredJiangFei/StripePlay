using Microsoft.Extensions.Options;
using Stripe;

DotNetEnv.Env.Load();
StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

StripeConfiguration.AppInfo = new AppInfo
{
    Name = "stripe-samples/accept-a-payment/payment-element",
    Url = "https://github.com/stripe-samples",
    Version = "0.1.0",
};

StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StripeOptions>(options =>
{
    options.PublishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
    options.SecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
    options.WebhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/config", (IOptions<StripeOptions> options) => new { options.Value.PublishableKey });

app.MapGet("/create-payment-intent", async () =>
{
    try
    {
        var customers = new CustomerService();
        var customer = customers.Create(new CustomerCreateOptions());

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(new PaymentIntentCreateOptions
        {
            Customer = customer.Id,
            SetupFutureUsage = "off_session",
            Amount = 1999,
            Currency = "EUR",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            }
            //PaymentMethodTypes = new List<string>
            //{
            //    "alipay",
            //    "card"
            //}
        });

        return Results.Ok(new { ClientSecret = paymentIntent.ClientSecret });
    }
    catch (StripeException e)
    {
        return Results.BadRequest(new { error = new { message = e.StripeError.Message } });
    }
});


//public void ChargeCustomer(string customerId)
//{
//    // Lookup the payment methods available for the customer
//    var paymentMethods = new PaymentMethodService();
//    var availableMethods = paymentMethods.List(new PaymentMethodListOptions
//    {
//        Customer = customerId,
//        Type = "card",
//    });
//    try
//    {
//        // Charge the customer and payment method immediately
//        var paymentIntentService = new PaymentIntentService();
//        var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
//        {
//            Amount = 1099,
//            Currency = "eur",
//            Customer = customerId,
//            PaymentMethod = availableMethods.Data[0].Id,
//            OffSession = true,
//            Confirm = true
//        });
//    }
//    catch (StripeException e)
//    {
//        switch (e.StripeError.ErrorType)
//        {
//            case "card_error":
//                // Error code will be authentication_required if authentication is needed
//                Console.WriteLine("Error code: " + e.StripeError.Code);
//                var paymentIntentId = e.StripeError.PaymentIntent.Id;
//                var service = new PaymentIntentService();
//                var paymentIntent = service.Get(paymentIntentId);

//                Console.WriteLine(paymentIntent.Id);
//                break;
//            default:
//                break;
//        }
//    }
//}


app.MapPost("/webhook", async (HttpRequest request, IOptions<StripeOptions> options) =>
{
    var json = await new StreamReader(request.Body).ReadToEndAsync();
    Event stripeEvent;
    try
    {
        stripeEvent = EventUtility.ConstructEvent(
            json,
            request.Headers["Stripe-Signature"],
             options.Value.WebhookSecret
        );
        app.Logger.LogInformation($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");
    }
    catch (Exception e)
    {
        app.Logger.LogInformation($"Something failed {e}");
        return Results.BadRequest();
    }

    if (stripeEvent.Type == Events.PaymentIntentSucceeded)
    {
        var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;
        app.Logger.LogInformation($"PaymentIntent ID: {paymentIntent.Id}");
        // Take some action based on the payment intent.
    }

    return Results.Ok();
});

app.Run();
