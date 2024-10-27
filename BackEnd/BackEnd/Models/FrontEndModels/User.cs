using BackEnd.Enums;

namespace BackEnd.Models.FrontEndModels
{
    public class User
    {
        public int userId { get; set; }
        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }

        public int cellPhoneNumber { get; set; }

        public DateTime regsitrationDate { get; set; }

        public DateTime birthDate { get; set; }

        public Gender gender { get; set; }
    }
}
