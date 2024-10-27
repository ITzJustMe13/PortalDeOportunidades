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
                oppportunity = OpportunityMapper.MapToDto(reservationModel.Opportunity),
                user = UserMapper.MapToDto(reservationModel.User),
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
            return new ReservationModel
            {
                reservationID = reservation.reservationId,
                Opportunity = OpportunityMapper.MapToModel(reservation.oppportunity),
                User = UserMapper.MapToModel(reservation.user),
                reservationDate = reservation.reservationDate,
                checkInDate = reservation.checkInDate,
                numOfPeople = reservation.numOfPeople,
                isActive = reservation.isActive,
                fixedPrice = reservation.fixedPrice
            };
        }
    }
}
