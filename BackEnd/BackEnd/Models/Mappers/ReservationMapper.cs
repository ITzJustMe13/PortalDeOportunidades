using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Mappers
{
    /// <summary>
    /// class that maps ReservationModel to Dto and vice-versa
    /// </summary>
    public class ReservationMapper
    {
        /// <summary>
        /// Function that maps ReservationModel parameters to Reservation parameters
        /// </summary>
        /// <param name="reservationModel"></param>
        /// <returns>Returns a Dto with the Model info</returns>
        public static Reservation? MapToDto(ReservationModel reservationModel) 
            {
            if (reservationModel == null)
                return null;

            var registrationDate = reservationModel.reservationDate == default ? DateTime.Now : reservationModel.reservationDate;

            return new Reservation
                {
                    reservationId = reservationModel.reservationID,
                    opportunityId = reservationModel.opportunityID,
                    userId = reservationModel.userID,
                    reservationDate = reservationModel.reservationDate,
                    date = reservationModel.Date,
                    numOfPeople = reservationModel.numOfPeople,
                    isActive = reservationModel.IsActive,
                    fixedPrice = reservationModel.fixedPrice
                };
            }

        /// <summary>
        /// Function that maps Reservation parameters to ReservationModel parameters
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns>Returns a Model with the Dto info</returns>
        public static ReservationModel? MapToModel(Reservation reservation)
            {
                if (reservation == null)
                    return null;

            if (reservation.reservationDate == default)
                reservation.reservationDate = DateTime.Now;
            
            var reservationModel = new ReservationModel
            {
                reservationID = reservation.reservationId,
                opportunityID = reservation.opportunityId,
                userID = reservation.userId,
                reservationDate = (DateTime)reservation.reservationDate!,
                Date = (DateTime)reservation.date!,
                numOfPeople = reservation.numOfPeople,
                IsActive = (bool)reservation.isActive!,
                fixedPrice = (float)reservation.fixedPrice!
            };

            ValidateModel(reservationModel);
            return reservationModel;
            }

        /// <summary>
        /// Function that validates if the mapping parameters are correct with each other
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="ValidationException"></exception>
        private static void ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, validateAllProperties: true))
            {
                // Se houver erros, lança uma exceção com detalhes
                var errorMessages = results.Select(r => r.ErrorMessage);
                throw new ValidationException("Erros de validação: " + string.Join("; ", errorMessages));
            }
        }   
    }
}
