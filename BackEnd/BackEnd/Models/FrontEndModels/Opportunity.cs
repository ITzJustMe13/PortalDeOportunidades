using BackEnd.Enums;

namespace BackEnd.Models.FrontEndModels
{
    public class Opportunity
    {
        public int opportunityId { get; set; }
        public string name { get; set;}

        public float price { get; set;}

        public int vacancies { get; set;}

        public bool isActive { get; set;}

        public Category category { get; set;}

        public string description { get; set;}

        public Location location { get; set;}

        public string address { get; set;}

        public User user { get; set;}

        public float reviewScore { get; set;}

        public DateTime date { get; set;}

        public bool isImpulsed { get; set;}
    }
}
