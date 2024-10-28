namespace BackEnd.Models.FrontEndModels
{
    public class Reservation
    {
        public int reservationId { get; set; }

        public int ppportunityId { get; set; }

        public int userId { get; set; }

        public DateTime reservationDate { get; set; }

        public DateTime checkInDate { get; set; }

        public int numOfPeople { get; set; }

        public bool isActive { get; set; }

        public float fixedPrice { get; set; }

    }
}
