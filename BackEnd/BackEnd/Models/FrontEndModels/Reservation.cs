using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.FrontEndModels
{
    public class Reservation
    {
        public int reservationId { get; set; }

        public int opportunityId { get; set; }

        public int userId { get; set; }

        public required DateTime reservationDate { get; set; }

        public required DateTime checkInDate { get; set; }

        public required int numOfPeople { get; set; }

        public required bool isActive { get; set; }

        public required float fixedPrice { get; set; }

    }
}
