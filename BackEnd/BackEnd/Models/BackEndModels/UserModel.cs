using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models.BackEndModels
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [MaxLength(200)]
        public string? HashedPassword { get; set; }

        public int? ExternalId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(9, ErrorMessage = "Cellphone Number should be 9 digits"),MinLength(9, ErrorMessage = "Cellphone Number should be 9 digits")]
        public int CellPhoneNum { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RegistrationDate { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [MaxLength(100)]
        public string Token { get; set; }

        public DateTime? TokenExpDate { get; set; }

        public ICollection<ReservationModel> Reservations { get; set; }

        public ICollection<OpportunityModel> Opportunities { get; set; }

        public ICollection<ImpulseModel> Impulses { get; set; }

        public ICollection<FavoritesModel> Favorites { get; set; }
    }
}
