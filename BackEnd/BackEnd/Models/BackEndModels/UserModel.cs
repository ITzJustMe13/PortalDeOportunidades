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

        [MaxLength(200, ErrorMessage = "O campo 'HashedPassword' deve ter no máximo 200 caracteres.")]
        public string? HashedPassword { get; set; }

        public int? ExternalId { get; set; }

        [Required(ErrorMessage = "O campo 'FirstName' é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo 'FirstName' deve ter no máximo 100 caracteres.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "O campo 'LastName' é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo 'LastName' deve ter no máximo 100 caracteres.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "O campo 'Email' é obrigatório.")]
        [EmailAddress(ErrorMessage ="O email tem um formato inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo 'CellPhoneNum' é obrigatório.")]
        [Range(100000000, 999999999, ErrorMessage = "O 'CellPhoneNum' deve ter 9 dígitos.")]
        public int CellPhoneNum { get; set; }

        [Required(ErrorMessage = "O campo 'RegistrationDate' é obrigatório.")]
        public DateTime RegistrationDate { get; set; }

        [Required(ErrorMessage = "O campo 'BirthDate' é obrigatório.")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "O campo 'Gender' é obrigatório.")]
        public Gender Gender { get; set; }

        [MaxLength(350, ErrorMessage = "O campo 'Token' deve ter no máximo 350 caracteres.")]
        public string? Token { get; set; }

        public bool isActive { get; set; }

        public DateTime? TokenExpDate { get; set; }

        public ICollection<ReservationModel> Reservations { get; set; }

        public ICollection<OpportunityModel> Opportunities { get; set; }

        public ICollection<ImpulseModel> Impulses { get; set; }

        public ICollection<FavoritesModel> Favorites { get; set; }
    }
}
