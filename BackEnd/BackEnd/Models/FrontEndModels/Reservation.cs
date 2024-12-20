﻿using BackEnd.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.FrontEndModels
{
    public class Reservation
    {
        public int reservationId { get; set; }

        public int opportunityId { get; set; }

        public int userId { get; set; }

        public DateTime? reservationDate { get; set; }

        public DateTime? date { get; set; }

        public required int numOfPeople { get; set; }

        public bool? isActive { get; set; }

        public float? fixedPrice { get; set; }

    }
}
