namespace BackEnd.Models.FrontEndModels
{
    public class Impulse
    {

        public User User { get; set; }

        public Opportunity Opportunity { get; set; }

        public float Value { get; set; }

        public DateTime ExpireDate { get; set; }


    }
}
