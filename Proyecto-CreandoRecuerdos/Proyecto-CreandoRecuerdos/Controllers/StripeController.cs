using Stripe;
using System.Web.Http;

public class PaymentIntentRequest
{
    public long Amount { get; set; }
}

public class StripeController : ApiController
{
    [HttpPost]
    [Route("api/payment/create-intent")]
    public IHttpActionResult CreateIntent([FromBody] PaymentIntentRequest request)
    {
        if (request == null || request.Amount <= 0)
        {
            return BadRequest("Monto inválido.");
        }

        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
        {
            Amount = request.Amount,  // monto en centavos
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            }
        });

        return Ok(new { clientSecret = paymentIntent.ClientSecret });
    }
}
