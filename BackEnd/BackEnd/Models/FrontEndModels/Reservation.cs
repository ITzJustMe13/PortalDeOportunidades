namespace BackEnd.Models.FrontEndModels
{
    public class Reservation
    {
        public int reservationId { get; set; }

        public Opportunity ppportunity { get; set; }

        public User user { get; set; }

        public DateTime reservationDate { get; set; }

        public DateTime checkInDate { get; set; }

        public int numOfPeople { get; set; }

        public bool isActive { get; set; }

        public float fixedPrice { get; set; }

    }
}
