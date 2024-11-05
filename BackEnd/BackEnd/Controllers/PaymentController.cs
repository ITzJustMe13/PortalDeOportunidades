using BackEnd.Models.FrontEndModels;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using Stripe.Events;
using Microsoft.Extensions.Options;
using BackEnd.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers.Data;

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

        // POST api/payment/Checkout-Reservation
        [HttpPost("Checkout-Reservation")]
        public async Task<IActionResult> CreateReservationCheckoutSession([FromBody] Reservation reservation)
        {
            // Validate reservation data
            if (reservation == null || reservation.fixedPrice <= 0)
            {
                return BadRequest("Invalid reservation data.");
            }

            // Retrieve user information
            var user = await _context.Users.FindAsync(reservation.userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Retrieve opportunity information
            var opportunity = await _context.Opportunities.FindAsync(reservation.opportunityId);
            if (opportunity == null)
            {
                return NotFound("Opportunity not found.");
            }

            // Calculate the total reservation amount in cents (for Stripe)
            var totalAmount = (long)(reservation.fixedPrice * reservation.numOfPeople * 100);

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
                    UnitAmount = (long)(reservation.fixedPrice * 100), // Price per person in cents
                    Currency = "eur",
                },
                Quantity = reservation.numOfPeople,
            },
        },
                Mode = "payment",
                CustomerEmail = user.Email,
                SuccessUrl = "http://localhost:7235/success",
                CancelUrl = "http://localhost:7235/cancel",
            };

            var service = new SessionService();
            Session session;
            try
            {
                session = await service.CreateAsync(options); // No StripeAccount specified, funds go to main account
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}");
            }

            return Ok(new { sessionId = session.Id });
        }



        // POST api/payment/Checkout-Impulse
        [HttpPost("Checkout-Impulse")]
        public async Task<IActionResult> CreateImpulseCheckoutSession([FromBody] Impulse impulse, int days)
        {
            // Validate Impulse Data
            if (impulse == null || impulse.value <= 0 || impulse.expireDate < DateTime.Today)
            {
                return BadRequest("Invalid impulse data.");
            }

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

            // Set up Session Options for Stripe
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
                        Name = $"Impulso para a visibiliade de {opportunity.Name} durante {days} dias.",
                    },
                    UnitAmount = (long)(impulse.value * 100), // Convert to cents
                    Currency = "eur",
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = "http://localhost:7235/success",  // Update with your frontend success page
                CancelUrl = "http://localhost:7235/cancel",    // Update with your frontend cancel page
                CustomerEmail = user.Email, // Send receipt to the user
            };

            var service = new SessionService();
            Session session;

            try
            {
                session = await service.CreateAsync(options); // Create the session
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}"); // Handle any Stripe exceptions
            }

            return Ok(new { sessionId = session.Id }); // Return sessionId for frontend redirection to Stripe
        }


        [HttpPost("stripe-webhook")]
        public async Task<IActionResult> StripeWebhook([FromServices] IConfiguration config)
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var webhookSecret = config["STRIPE_WEBHOOK_SECRET"]; // Access from env

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], webhookSecret);
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook Error: {ex.Message}");
            }

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;

                // Process the session, update reservation status, etc.
            }

            return Ok();
        }

    }
}
