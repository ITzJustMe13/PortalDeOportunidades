using BackEnd.ServiceResponses;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Interfaces
{
    /// <summary>
    /// This interface is responsibile for all the functions of the logic part of Reservations
    /// </summary>
    public interface IReservationService
    {

        Task<ServiceResponse<IEnumerable<Reservation>>> GetAllActiveReservationsByUserIdAsync(int userId);

        Task<ServiceResponse<IEnumerable<Reservation>>> GetAllReservationsByUserIdAsync(int userId);

        Task<ServiceResponse<Reservation>> GetReservationByIdAsync(int id);

        Task<ServiceResponse<Reservation>> CreateNewReservationAsync(Reservation reservation);

        Task<ServiceResponse<bool>> DeactivateReservationByIdAsync(int id);

        Task<ServiceResponse<bool>> UpdateReservationAsync(int id, Reservation reservation);
        
        Task<ServiceResponse<bool>> DeleteReservationAsync(int id);
    }
}
