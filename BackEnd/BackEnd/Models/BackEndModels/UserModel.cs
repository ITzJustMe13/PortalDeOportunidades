using System.ComponentModel.DataAnnotations;
using BackEnd.Enums;

namespace BackEnd.Models.BackEndModels
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [MaxLength(200, ErrorMessage = "The field 'HashedPassword' has to be less than 200 characters.")]
        public string? HashedPassword { get; set; }

        public int? ExternalId { get; set; }

        [Required(ErrorMessage = "The field 'FirstName' is required.")]
        [MaxLength(100, ErrorMessage = "The field 'FirstName' has to be less than 100 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The field 'LastName' is required.")]
        [MaxLength(100, ErrorMessage = "The field 'LastName' has to be less than 100 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The campo 'Email' is required.")]
        [EmailAddress(ErrorMessage ="The email has an invalid format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The field 'CellPhoneNum' is required.")]
        [Range(100000000, 999999999, ErrorMessage = "The field 'CellPhoneNum' has to be 9 digits.")]
        public int CellPhoneNum { get; set; }

        [Required(ErrorMessage = "The field 'RegistrationDate' is required.")]
        public DateTime RegistrationDate { get; set; }

        [Required(ErrorMessage = "The field 'BirthDate' is required.")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "The field 'Gender' is required.")]
        public Gender Gender { get; set; }

        [MaxLength(350, ErrorMessage = "The field 'Token' has to be less than 350 characters.")]
        public string? Token { get; set; }

        [Required( ErrorMessage = "The field 'isActive' is required.")]
        public bool isActive { get; set; }
        [Required(ErrorMessage = "The field 'Image' is required.")]
        public byte[] Image { get; set; }

        public DateTime? TokenExpDate { get; set; }

        public string? IBAN { get; set; }

        public ICollection<ReservationModel> Reservations { get; set; }

        public ICollection<OpportunityModel> Opportunities { get; set; }

        public ICollection<ImpulseModel> Impulses { get; set; }

        public ICollection<FavoritesModel> Favorites { get; set; }
    }
}
