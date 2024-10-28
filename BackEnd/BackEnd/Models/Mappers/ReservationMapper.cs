using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Models.Mappers
{
        public class ReservationMapper
        {
            public static Reservation MapToDo(ReservationModel reservationModel) 
            {
                if (reservationModel == null)
                    throw new ArgumentNullException(nameof(reservationModel));
                return new Reservation
                {
                    reservationId = reservationModel.reservationID,
                    oppportunityId = reservationModel.opportunityID,
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
            
                return new ReservationModel
                {
                    reservationID = reservation.reservationId,
                    opportunityID = reservation.oppportunityId,
                    userID = reservation.userId,
                    reservationDate = reservation.reservationDate,
                    checkInDate = reservation.checkInDate,
                    numOfPeople = reservation.numOfPeople,
                    isActive = reservation.isActive,
                    fixedPrice = reservation.fixedPrice
                };
            }
        }
}
