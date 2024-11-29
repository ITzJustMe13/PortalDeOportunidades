using BackEnd.Models.FrontEndModels;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using BackEnd.Controllers.Data;
using BackEnd.Services;
using BackEnd.Interfaces;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Endpoint that creates a checkout session using strip for paying for a reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns>The checkout Session id for redirection on stripe</returns>
        [HttpPost("Checkout-Reservation")]
        public async Task<IActionResult> CreateReservationCheckoutSession([FromBody] Reservation reservation)
        {
            if (_context == null)
                return NotFound("DB context missing");

            if (reservation == null || reservation.fixedPrice <= 0)
            {
                return BadRequest("Invalid reservation data.");
            }


            // User information
            var user = await _context.Users.FindAsync(reservation.userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Opportunity information
            var opportunity = await _context.Opportunities.FindAsync(reservation.opportunityId);
            if (opportunity == null)
            {
                return NotFound("Opportunity not found.");
            }


            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Reserva para {reservation.numOfPeople} pessoas para {opportunity.Name} ({opportunity.OpportunityId})",
                    },
                    UnitAmount = (long)(reservation.fixedPrice * 100), // Price of the reservation in cents for stripe
                    Currency = "eur",
                },
                Quantity = reservation.numOfPeople,
            },
        },
                Mode = "payment",
                SuccessUrl = $"http://localhost:50394/#/payment/success?paymentType=reservation",
                CancelUrl = $"http://localhost:50394/#/payment/cancel?paymentType=reservation",
                CustomerEmail = user.Email, // For sending the receipt to the user
            };

            var service = new SessionService();
            Session session;
            try
            {
                session = await service.CreateAsync(options);
                
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}");
            }

            return Ok(new { sessionId = session.Id });
        }




        /// <summary>
        /// Endpoint that creates a checkout session using strip for paying for a Opportunity Impulse
        /// </summary>
        /// <param name="impulse"></param>
        /// <returns>The checkout Session id for redirection on stripe</returns>
        [HttpPost("Checkout-Impulse")]
        public async Task<IActionResult> CreateImpulseCheckoutSession([FromBody] Impulse impulse)
        {

            if (impulse == null || impulse.value <= 0 || impulse.expireDate <= DateTime.Today)
            {
                return BadRequest("Invalid impulse data.");
            }

            if (_context == null)
                return NotFound("DB context missing");

            // Find the User
            var user = await _context.Users.FindAsync(impulse.userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Find the Opportunity
            var opportunity = await _context.Opportunities.FindAsync(impulse.opportunityId);
            if (opportunity == null)
            {
                return NotFound("Opportunity not found.");
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Impulso para a visibiliade de {opportunity.Name} até ao dia {impulse.expireDate} .",
                    },
                    UnitAmount = (long)(impulse.value * 100), // Convert to cents for stripe
                    Currency = "eur",
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = $"http://localhost:50394/#/payment/success?paymentType=impulse&entity={impulse}",
                CancelUrl = $"http://localhost:50394/#/payment/cancel?paymentType=impulse&entity={impulse}",
                CustomerEmail = user.Email, // For sending the receipt to the user
            };

            var service = new SessionService();
            Session session;

            try
            {
                session = await service.CreateAsync(options); // Create the session for checkout
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}"); // Handle any Stripe exceptions
            }

            return Ok(new { sessionId = session.Id }); // Return sessionId for frontend redirection to Stripe
        }


    }
}
