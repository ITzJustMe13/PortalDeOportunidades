using BackEnd.GenericClasses;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Interfaces
{
    public interface IPaymentService
    {
        Task<ServiceResponse<string>> CreateReservationCheckoutSessionAsync(Reservation reservation);
    }
}
