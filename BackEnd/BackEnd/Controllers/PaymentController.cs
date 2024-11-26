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
    public class PaymentController : BaseController
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

            return HandleResponse(serviceResponse);
        }


        // POST api/payment/Checkout-Impulse
        [HttpPost("Checkout-Impulse")]
        public async Task<IActionResult> CreateImpulseCheckoutSession([FromBody] Impulse impulse)
        {
            var serviceResponse = await _paymentService.CreateImpulseCheckoutSessionAsync(impulse);

            return HandleResponse(serviceResponse);
        }

    }
}
