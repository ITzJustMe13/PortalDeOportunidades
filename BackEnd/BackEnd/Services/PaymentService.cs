using BackEnd.GenericClasses;
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
    }
}
