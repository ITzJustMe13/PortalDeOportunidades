using BackEnd.Enums;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.FrontEndModels
{
    public class User
    {
        public int userId { get; set; }

        [Required]
        public string? password { get; set; }

        [Required]
        public string? firstName { get; set; }

        [Required]
        public string? lastName { get; set; }

        [Required]
        public string? email { get; set; }

        [Required]
        public int? cellPhoneNumber { get; set; }

        public DateTime registrationDate { get; set; }

        [Required]
        public DateTime? birthDate { get; set; }

        [Required]
        public Gender? gender { get; set; }
    }
}
