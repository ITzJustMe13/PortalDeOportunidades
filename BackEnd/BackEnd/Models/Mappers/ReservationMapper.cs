using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Mappers
{
        public class ReservationMapper
        {
            public static Reservation MapToDto(ReservationModel reservationModel) 
            {
                if (reservationModel == null)
                    throw new ArgumentNullException(nameof(reservationModel));

                ValidateModel(reservationModel);

            var registrationDate = reservationModel.reservationDate == default ? DateTime.Now : reservationModel.reservationDate;

            return new Reservation
                {
                    reservationId = reservationModel.reservationID,
                    opportunityId = reservationModel.opportunityID,
                    userId = reservationModel.userID,
                    reservationDate = reservationModel.reservationDate,
                    checkInDate = reservationModel.checkInDate,
                    numOfPeople = reservationModel.numOfPeople,
                    isActive = reservationModel.isActive,
                    fixedPrice = reservationModel.fixedPrice
                };
            }

            public static ReservationModel MapToModel(Reservation reservation)
            {
                if (reservation == null)
                    throw new ArgumentNullException(nameof(reservation));

            if (reservation.reservationDate == default)
                reservation.reservationDate = DateTime.Now;
            
                var reservationModel = new ReservationModel
                {
                    reservationID = reservation.reservationId,
                    opportunityID = reservation.opportunityId,
                    userID = reservation.userId,
                    reservationDate = reservation.reservationDate,
                    checkInDate = reservation.checkInDate,
                    numOfPeople = reservation.numOfPeople,
                    isActive = reservation.isActive,
                    fixedPrice = reservation.fixedPrice
                };
            ValidateModel(reservationModel);
            return reservationModel;
            }

        // Método para validar o modelo usando DataAnnotations
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
