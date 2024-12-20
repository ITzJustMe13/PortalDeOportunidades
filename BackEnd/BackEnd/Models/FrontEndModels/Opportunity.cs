﻿using BackEnd.Enums;
using BackEnd.Interfaces;

namespace BackEnd.Models.FrontEndModels
{
    public class Opportunity
    {
        public int opportunityId { get; set; }
        public required string name { get; set;}

        public required decimal price { get; set;}

        public required int vacancies { get; set;}

        public bool isActive { get; set;}

        public required Category category { get; set;}

        public required string description { get; set;}

        public required Location location { get; set;}

        public required string address { get; set;}

        public required int userId { get; set;}

        public float reviewScore { get; set;}

        public required DateTime date { get; set;}

        public required bool isImpulsed { get; set;}

        public ICollection<OpportunityImg?> OpportunityImgs { get; set; }
    }
}
