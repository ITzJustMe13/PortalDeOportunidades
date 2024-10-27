namespace BackEnd.Models.FrontEndModels
{
    public class Reservation
    {
        public int ReservationId { get; set; }

        public Opportunity Opportunity { get; set; }

        public User User { get; set; }

        public DateTime ReservationDate { get; set; }

        public DateTime CheckInDate { get; set; }

        public int NumOfPeople { get; set; }

        public bool IsActive { get; set; }

        public float FixedPrice { get; set; }

    }
}
