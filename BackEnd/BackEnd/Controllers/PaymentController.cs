using BackEnd.Models.FrontEndModels;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using BackEnd.Controllers.Data;
using BackEnd.Interfaces;
using BackEnd.Services;

namespace BackEnd.Controllers
{
    /// <summary>
    /// Controller Responsible for Payments
    /// Has a constructor that receives an IPaymentService
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;


        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        /// <summary>
        /// Endpoint that creates a Checkout Session for a User Reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPost("Checkout-Reservation")]
        public async Task<IActionResult> CreateReservationCheckoutSession([FromBody] Reservation reservation)
        {
            var serviceResponse = await _paymentService.CreateReservationCheckoutSessionAsync(reservation);

            return HandleResponse(serviceResponse);
        }


        /// <summary>
        /// Endpoint that creates a Checkout Session for a Opportunity Impulse payment
        /// </summary>
        /// <param name="impulse"></param>
        /// <returns></returns>
        [HttpPost("Checkout-Impulse")]
        public async Task<IActionResult> CreateImpulseCheckoutSession([FromBody] Impulse impulse)
        {
            var serviceResponse = await _paymentService.CreateImpulseCheckoutSessionAsync(impulse);

            return HandleResponse(serviceResponse);
        }

    }
}
