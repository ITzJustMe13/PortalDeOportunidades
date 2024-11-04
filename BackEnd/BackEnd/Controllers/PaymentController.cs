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
            if (reservation == null || reservation.fixedPrice <= 0)
            {
                return BadRequest("Invalid reservation data.");
            }

            var user = await _context.Users.FindAsync(reservation.userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var opportunity = await _context.Opportunities.FindAsync(reservation.opportunityId);
            if (opportunity == null)
            {
                return NotFound("Opportunity not found.");
            }

            var serviceProvider = await _context.Users.FindAsync(opportunity.userID);
            if (serviceProvider == null || string.IsNullOrEmpty(serviceProvider.StripeAccountId))
            {
                return BadRequest("Service provider is not registered for payments.");
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
                        Name = $"Reservation for {reservation.numOfPeople} people for {opportunity.Name}",
                    },
                    UnitAmount = (long)(reservation.fixedPrice * 100), // Convert to cents
                    Currency = "eur",
                },
                Quantity = reservation.numOfPeople,
            },
        },
                Mode = "payment",
                CustomerEmail = user.Email,
                SuccessUrl = "https://your-website.com/success",
                CancelUrl = "https://your-website.com/cancel",
            };

            // Pass the service provider's Stripe account for the payment
            var service = new SessionService();
            Session session;
            try
            {
                session = await service.CreateAsync(options, new RequestOptions
                {
                    StripeAccount = serviceProvider.StripeAccountId
                });
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
                        Name = $"Impulse for {opportunity.Name} for {days} days visibility boost",
                    },
                    UnitAmount = (long)(impulse.value * 100), // Convert to cents
                    Currency = "eur",
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = "https://your-website.com/success",  // Update with your frontend success page
                CancelUrl = "https://your-website.com/cancel",    // Update with your frontend cancel page
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
