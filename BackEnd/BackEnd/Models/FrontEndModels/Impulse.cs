namespace BackEnd.Models.FrontEndModels
{
    public class Impulse
    {

        public User user { get; set; }

        public Opportunity opportunity { get; set; }

        public float value { get; set; }

        public DateTime expireDate { get; set; }


    }
}
