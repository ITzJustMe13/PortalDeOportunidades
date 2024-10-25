﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.BackEndModels
{
    public class ReservationModel
    {

        [Key]
        public int reservationID {  get; set; }

        [ForeignKey("OpportunityModel")]
        public int opportunityID { get; set; }
        public virtual OpportunityModel Opportunity { get; set; }

        [ForeignKey("User")]
        public int userID { get; set; }
        public virtual UserModel User { get; set; }

        [Required]
        public DateTime reservationDate { get; set; }

        [Required]
        public DateTime checkInDate { get; set; }

        [Required]
        public int numOfPeople { get; set; }

        [Required]
        public bool isActive  { get; set; }

    }
}
