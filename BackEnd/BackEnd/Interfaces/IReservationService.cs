﻿using BackEnd.GenericClasses;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Interfaces
{
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