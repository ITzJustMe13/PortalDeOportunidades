using BackEnd.ServiceResponses;
using BackEnd.Interfaces;
using BackEnd.Models.FrontEndModels;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using Stripe;
using BackEnd.Controllers.Data;

namespace BackEnd.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext dbContext;

        public PaymentService(
            ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<ServiceResponse<string>> CreateReservationCheckoutSessionAsync(Reservation reservation)
        {
            var response = new ServiceResponse<string>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (reservation == null || reservation.fixedPrice <= 0)
                {
                    response.Success = false;
                    response.Message = "Invalid reservation data.";
                    response.Type = "BadRequest";
                    return response;
                }

                // User information
                var user = await dbContext.Users.FindAsync(reservation.userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    response.Type = "NotFound";
                    return response;
                }

                // Opportunity information
                var opportunity = await dbContext.Opportunities.FindAsync(reservation.opportunityId);
                if (opportunity == null)
                {
                    response.Success = false;
                    response.Message = "Opportunity not found.";
                    response.Type = "NotFound";
                    return response;
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
                        UnitAmount = (long)(reservation.fixedPrice * 100), // Price of the reservation in cents for Stripe
                        Currency = "eur",
                    },
                    Quantity = reservation.numOfPeople,
                },
            },
                    Mode = "payment",
                    SuccessUrl = "https://localhost:7235/success", // UPDATE WITH FRONTEND
                    CancelUrl = "https://localhost:7235/cancel", // UPDATE WITH FRONTEND
                    CustomerEmail = user.Email, // For sending the receipt to the user
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                response.Success = true;
                response.Data = session.Id; // Returning the session ID
                response.Message = "Checkout session created successfully.";
                response.Type = "Ok";
            }
            catch (StripeException ex)
            {
                response.Success = false;
                response.Message = $"Stripe error: {ex.Message}";
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while creating the checkout session.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        public async Task<ServiceResponse<string>> CreateImpulseCheckoutSessionAsync(Impulse impulse)
        {
            var response = new ServiceResponse<string>();

            try
            {
                if (impulse == null || impulse.value <= 0 || impulse.expireDate <= DateTime.Today)
                {
                    response.Success = false;
                    response.Message = "Invalid impulse data.";
                    response.Type = "BadRequest";
                    return response;
                }

                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context missing.";
                    response.Type = "NotFound";
                    return response;
                }

                // Find the User
                var user = await dbContext.Users.FindAsync(impulse.userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    response.Type = "NotFound";
                    return response;
                }

                // Find the Opportunity
                var opportunity = await dbContext.Opportunities.FindAsync(impulse.opportunityId);
                if (opportunity == null)
                {
                    response.Success = false;
                    response.Message = "Opportunity not found.";
                    response.Type = "NotFound";
                    return response;
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
                            Name = $"Impulso para a visibilidade de {opportunity.Name} até ao dia {impulse.expireDate}.",
                        },
                        UnitAmount = (long)(impulse.value * 100), // Convert to cents for stripe
                        Currency = "eur",
                    },
                    Quantity = 1,
                },
            },
                    Mode = "payment",
                    SuccessUrl = "https://localhost:7235/success", // UPDATE WITH FRONTEND
                    CancelUrl = "https://localhost:7235/cancel",  // UPDATE WITH FRONTEND
                    CustomerEmail = user.Email, // For sending the receipt to the user
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options); // Create the session for checkout

                response.Data = session.Id;
                response.Success = true;
                response.Message = "Stripe session created successfully.";
                response.Type = "Ok";
            }
            catch (StripeException ex)
            {
                response.Success = false;
                response.Message = $"Stripe error: {ex.Message}";
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred while creating the payment session.";
                response.Type = "InternalServerError";
            }

            return response;
        }
    }
}
