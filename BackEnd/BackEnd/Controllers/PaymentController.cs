using BackEnd.Models.FrontEndModels;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
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
                            Name = $"Reserva para {reservation.numOfPeople} para {opportunity.Name}",
                        },
                        UnitAmount = (long)(reservation.fixedPrice * 100), // Convert to cents because Stripe wants the smallest monetary size
                        Currency = "eur", // Adjust as necessary
                    },
                    Quantity = reservation.numOfPeople, // Assuming one reservation per session
                },
            },
                Mode = "payment",
                /* NECESSITA DE SER ALTERADO MEDIANTE O FRONT END
                SuccessUrl = "https://your-website.com/success",
                CancelUrl = "https://your-website.com/cancel",
                */
                CustomerEmail = user.Email, // Send receipt to this email
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

        // POST api/payment/Checkout-Impulse
        [HttpPost("Checkout-Impulse")]
        public async Task<IActionResult> CreateImpulseCheckoutSession([FromBody] Impulse impulse, int days)
        {
            if (impulse == null || impulse.value <= 0 || impulse.expireDate < DateTime.Today)
            {
                return BadRequest("Invalid impulse data.");
            }

            var user = await _context.Users.FindAsync(impulse.userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
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
                            Name = $"Impulso para a Oportunidade {opportunity.Name} por {days} dias",
                        },
                        UnitAmount = (long)(impulse.value * 100), // Convert to cents because Stripe wants the smallest monetary size
                        Currency = "eur", // Adjust as necessary
                    },
                    Quantity = 1, // Assuming one reservation per session
                },
            },
                Mode = "payment",
                /* NECESSITA DE SER ALTERADO MEDIANTE O FRONT END
                SuccessUrl = "https://your-website.com/success",
                CancelUrl = "https://your-website.com/cancel",
                */
                CustomerEmail = user.Email, // Send receipt to this email
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

    }
}
