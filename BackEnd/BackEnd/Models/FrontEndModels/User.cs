using BackEnd.Enums;

namespace BackEnd.Models.FrontEndModels
{
    public class User
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int CellphoneNumber { get; set; }

        public DateTime RegsitrationDate { get; set; }

        public DateTime BirthDate { get; set; }

        public Gender Gender { get; set; }
    }
}
