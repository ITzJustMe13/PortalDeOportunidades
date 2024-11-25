using BackEnd.Models.FrontEndModels;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using BackEnd.Controllers.Data;
using BackEnd.Interfaces;
using BackEnd.Services;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;


        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }


        [HttpPost("Checkout-Reservation")]
        public async Task<IActionResult> CreateReservationCheckoutSession([FromBody] Reservation reservation)
        {
            var serviceResponse = await _paymentService.CreateReservationCheckoutSessionAsync(reservation);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    "InternalServerError" => StatusCode(500, serviceResponse.Message),
                    _ => StatusCode(500, serviceResponse.Message)
                };
            }

            return Ok(new { sessionId = serviceResponse.Data }); // Return the session ID
        }


        // POST api/payment/Checkout-Impulse
        [HttpPost("Checkout-Impulse")]
        public async Task<IActionResult> CreateImpulseCheckoutSession([FromBody] Impulse impulse)
        {
            var serviceResponse = await _paymentService.CreateImpulseCheckoutSessionAsync(impulse);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    "NotFound" => NotFound(serviceResponse.Message),
                    "InternalServerError" => StatusCode(500, serviceResponse.Message),
                    _ => StatusCode(500, "Unknown error.")
                };
            }

            return Ok(new { sessionId = serviceResponse.Data });
        }

    }
}
