using BackEnd.Enums;

namespace BackEnd.Models.FrontEndModels
{
    public class User
    {
        public int userId { get; set; }

        public required string? password { get; set; }

        public required string firstName { get; set; }

        public required string lastName { get; set; }

        public required string email { get; set; }

        public required int cellPhoneNumber { get; set; }

        public DateTime registrationDate { get; set; }

        public required DateTime birthDate { get; set; }

        public required Gender gender { get; set; }

        public byte[] image { get; set; }
    }
}
