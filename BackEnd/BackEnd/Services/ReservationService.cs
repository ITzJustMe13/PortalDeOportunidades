using BackEnd.Controllers.Data;
using BackEnd.ServiceResponses;
using BackEnd.Interfaces;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Services
{
    /// <summary>
    /// This class is responsible for the Reservation logic of the program
    /// and implements the IReservationService Interface
    /// Has a constructor that receives a DBContext
    /// </summary>
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext dbContext;

        public ReservationService(
            ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Function that gets all active Reservations from a user by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and the Reservations Dtos</returns>
        public async Task<ServiceResponse<IEnumerable<Reservation>>> GetAllActiveReservationsByUserIdAsync(int userId)
        {
            var response = new ServiceResponse<IEnumerable<Reservation>>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context missing.";
                    response.Type = "NotFound";
                    return response;
                }

                var reservations = await dbContext.Reservations
                    .Where(r => r.userID == userId && r.isActive)
                    .ToListAsync();

                if (!reservations.Any())
                {
                    response.Success = false;
                    response.Message = "No active reservations found for the specified user.";
                    response.Type = "NotFound";
                    return response;
                }

                var reservationDtos = reservations.Select(r => ReservationMapper.MapToDto(r));
                response.Success = true;
                response.Data = reservationDtos;
                response.Message = "Active reservations retrieved successfully.";
                response.Type = "Ok";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while fetching active reservations.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        /// <summary>
        /// Function that gets all the Reservations of a user by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and the Reservation Dtos</returns>
        public async Task<ServiceResponse<IEnumerable<Reservation>>> GetAllReservationsByUserIdAsync(int userId)
        {
            var response = new ServiceResponse<IEnumerable<Reservation>>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                var reservations = await dbContext.Reservations
                    .Where(r => r.User.UserId == userId)
                    .ToListAsync();

                if (!reservations.Any())
                {
                    response.Success = false;
                    response.Message = "No reservations found for the specified user.";
                    response.Type = "NotFound";
                    return response;
                }

                var reservationDtos = reservations.Select(r => ReservationMapper.MapToDto(r)).ToList();

                response.Data = reservationDtos;
                response.Success = true;
                response.Message = "Reservations retrieved successfully.";
                response.Type = "Ok";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred while retrieving reservations.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        /// <summary>
        /// Function that get a Reservation by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and the Reservation Dto</returns>
        public async Task<ServiceResponse<Reservation>> GetReservationByIdAsync(int id)
        {
            var response = new ServiceResponse<Reservation>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context missing.";
                    response.Type = "NotFound";
                    return response;
                }

                var reservation = await dbContext.Reservations.FindAsync(id);

                if (reservation == null)
                {
                    response.Success = false;
                    response.Message = "Reservation not found.";
                    response.Type = "NotFound";
                    return response;
                }

                var reservationDto = ReservationMapper.MapToDto(reservation);

                response.Success = true;
                response.Data = reservationDto;
                response.Message = "Reservation retrieved successfully.";
                response.Type = "Ok";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while retrieving the reservation.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        /// <summary>
        /// Function that creates a new Reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true, creates the reservation
        /// and returns the created Review Dto</returns>
        public async Task<ServiceResponse<Reservation>> CreateNewReservationAsync(Reservation reservation)
        {
            var response = new ServiceResponse<Reservation>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context missing.";
                    response.Type = "NotFound";
                    return response;
                }

                // Verifica se os campos obrigatórios estão presentes e válidos
                if (reservation == null || reservation.opportunityId == 0 || reservation.userId == 0)
                {
                    response.Success = false;
                    response.Message = "Some required fields are missing or invalid.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Verifica se a oportunidade e o usuário existem
                var opportunity = await dbContext.Opportunities.FindAsync(reservation.opportunityId);
                var user = await dbContext.Users.FindAsync(reservation.userId);

                if (opportunity == null)
                {
                    response.Success = false;
                    response.Message = "Opportunity not found.";
                    response.Type = "NotFound";
                    return response;
                }

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    response.Type = "NotFound";
                    return response;
                }

                // Verifica a validade do número de pessoas
                if (reservation.numOfPeople < 0)
                {
                    response.Success = false;
                    response.Message = "The value Number Of People must be valid";
                    response.Type = "BadRequest";
                    return response;
                }

                // Verifica se o número de pessoas não excede o número de vagas
                if (reservation.numOfPeople > opportunity.Vacancies)
                {
                    response.Success = false;
                    response.Message = "The numberOfPeople is bigger than number of vacancies";
                    response.Type = "BadRequest";
                    return response;
                }

                reservation.reservationDate = DateTime.Now;
                reservation.checkInDate = opportunity.Date;
                reservation.isActive = true;
                reservation.fixedPrice = ((float)(reservation.numOfPeople * opportunity.Price));

                var reservationModel = ReservationMapper.MapToModel(reservation);
                dbContext.Reservations.Add(reservationModel);
                await dbContext.SaveChangesAsync();

                var reservationDto = ReservationMapper.MapToDto(reservationModel);

                response.Success = true;
                response.Message = "Reservation created successfully.";
                response.Type = "Created";
                response.Data = reservationDto;

            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while creating the reservation.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        /// <summary>
        /// Function that deactivates the reservation by its id
        /// </summary>
        /// <param name="id">Id of the reservation</param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and deactivates the reservation</returns>
        public async Task<ServiceResponse<bool>> DeactivateReservationByIdAsync(int id)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context missing.";
                    response.Type = "NotFound";
                    return response;
                }

                var reservationModel = await dbContext.Reservations.FindAsync(id);
                if (reservationModel == null)
                {
                    response.Success = false;
                    response.Message = $"Reservation with id {id} not found.";
                    response.Type = "NotFound";
                    return response;
                }

                // Verifica se a reserva pode ser desativada
                if (reservationModel.checkInDate > DateTime.Now && reservationModel.isActive)
                {
                    reservationModel.isActive = false;
                    await dbContext.SaveChangesAsync();
                    response.Success = true;
                    response.Message = "Reservation successfully deactivated.";
                    response.Type = "Ok";
                    response.Data = true;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Reservation is impossible to deactivate";
                    response.Type = "BadRequest";
                    response.Data = false;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while deactivating the reservation.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        /// <summary>
        /// Function that updates the reservation by its id and an updated reservation dto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reservation"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true, updates the reservation and
        /// returns the updated reservation dto</returns>
        public async Task<ServiceResponse<bool>> UpdateReservationAsync(int id, Reservation reservation)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context missing.";
                    response.Type = "NotFound";
                    return response;
                }

                // Verifica se a reserva e a oportunidade existem
                var existingReservation = await dbContext.Reservations.FindAsync(id);
                var opportunity = await dbContext.Opportunities.FindAsync(reservation.opportunityId);

                if (existingReservation == null)
                {
                    response.Success = false;
                    response.Message = "Reservation not found.";
                    response.Type = "NotFound";
                    return response;
                }

                if (reservation.numOfPeople < 0)
                {
                    response.Success = false;
                    response.Message = "The value Number Of People must be valid.";
                    response.Type = "BadRequest";
                    return response;
                }

                if (reservation.numOfPeople > opportunity.Vacancies)
                {
                    response.Success = false;
                    response.Message = "The numberOfPeople is bigger than number of vacancies.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Atualiza as propriedades da reserva
                existingReservation.numOfPeople = reservation.numOfPeople;
                existingReservation.fixedPrice = (float)(reservation.numOfPeople * opportunity.Price);

                dbContext.Entry(existingReservation).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();

                response.Success = true;
                response.Message = "Reservation successfully updated.";
                response.Type = "Ok";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while updating the reservation.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        /// <summary>
        /// Function that deletes the reservation by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and deletes the reservation</returns>
        public async Task<ServiceResponse<bool>> DeleteReservationAsync(int id)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context missing.";
                    response.Type = "NotFound";
                    return response;
                }

                // Busca a reserva para excluir
                var reservationToDelete = await dbContext.Reservations.FindAsync(id);

                if (reservationToDelete == null)
                {
                    response.Success = false;
                    response.Message = "Reservation not found.";
                    response.Type = "NotFound";
                    return response;
                }

                // Exclui a reserva
                dbContext.Reservations.Remove(reservationToDelete);
                await dbContext.SaveChangesAsync();

                response.Success = true;
                response.Message = "Reservation successfully deleted.";
                response.Type = "Ok";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while deleting the reservation.";
                response.Type = "InternalServerError";
            }

            return response;
        }

    }
}
