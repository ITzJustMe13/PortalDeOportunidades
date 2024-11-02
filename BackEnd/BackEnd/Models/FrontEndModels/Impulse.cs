namespace BackEnd.Models.FrontEndModels
{
    public class Impulse
    {

        public int userId { get; set; }

        public int opportunityId { get; set; }

        public required float value { get; set; }

        public required DateTime expireDate { get; set; }
    }
}
