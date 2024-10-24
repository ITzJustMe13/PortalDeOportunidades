﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Enums;

namespace BackEnd.Models.BackEndModels
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [MaxLength(200)]
        public string HashedPassword { get; set; }

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
        [MaxLength(15)]
        public string CellPhoneNum { get; set; }

        [Required]
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
    }
}
