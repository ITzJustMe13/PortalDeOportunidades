﻿namespace BackEnd.Models.FrontEndModels
{
    public class Review
    {
        public required int reservationId { get; set; }

        public required float rating { get; set; }

        public required string? desc { get; set; }
    }
}
