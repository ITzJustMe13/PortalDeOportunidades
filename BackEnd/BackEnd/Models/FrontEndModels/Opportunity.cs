using BackEnd.Enums;

namespace BackEnd.Models.FrontEndModels
{
    public class Opportunity
    {
        public string Name { get; set;}

        public float Price { get; set;}

        public int Vacancies { get; set;}

        public bool IsActive { get; set;}

        public Category Category { get; set;}

        public string Description { get; set;}

        public Location Location { get; set;}

        public string Address { get; set;}

        public User User { get; set;}

        public float ReviewScore { get; set;}

        public DateTime Date { get; set;}

        public bool IsImpulsed { get; set;}
    }
}
