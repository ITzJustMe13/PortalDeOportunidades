namespace BackEnd.Models.FrontEndModels
{
    public class Review
    {
        public Reservation reservation { get; set; }

        public float rating { get; set; }

        public string desc { get; set; }
    }
}
