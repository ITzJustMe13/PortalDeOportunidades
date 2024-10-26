namespace BackEnd.Models.FrontEndModels
{
    public class Review
    {
        public Reservation Reservation { get; set; }

        public float Rating { get; set; }

        public string Desc { get; set; }
    }
}
