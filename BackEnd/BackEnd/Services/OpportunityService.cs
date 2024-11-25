using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.GenericClasses;
using BackEnd.Interfaces;
using BackEnd.Models.FrontEndModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace BackEnd.Services
{
    /// <summary>
    /// This class is responsible for the Opportunity logic of the program
    /// and implements the IOpportunityService Interface
    /// Has a constructor that receives a DBContext
    /// </summary>
    public class OpportunityService : IOpportunityService
    {
        private readonly ApplicationDbContext dbContext;

        public OpportunityService(ApplicationDbContext context)
        {
            dbContext = context;
        }

        /// <summary>
        /// Function that validates the opportunity search parameters
        /// </summary>
        /// <param name="vacancies"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns>Returns a List of Strings (errors) or a empty List based 
        /// of the validation of the parameters </returns>
        public List<string> ValidateSearchParameters(int? vacancies, decimal? minPrice, decimal? maxPrice, Category? category, Location? location)
        {
            var errors = new List<string>();

            if (vacancies.HasValue && vacancies <= 0)
                errors.Add("Vacancies must be greater than zero.");

            if (minPrice.HasValue && minPrice <= 0.00M)
                errors.Add("MinPrice should be greater than 0.01.");

            if (maxPrice.HasValue && maxPrice <= 0.00M)
                errors.Add("MaxPrice should be greater than 0.01.");

            if (minPrice.HasValue && maxPrice.HasValue)
            {
                if (minPrice > maxPrice)
                {
                    errors.Add("MaxPrice should be greater than MinPrice.");
                }
            }

            if (category.HasValue && !Enum.IsDefined(typeof(Category), category.Value))
                errors.Add("Invalid category specified.");

            if (location.HasValue && !Enum.IsDefined(typeof(Location), location.Value))
                errors.Add("Invalid location specified.");

            return errors;
        }

        /// <summary>
        /// Function that validates the Opportunity parameters for creating or editing an Opportunity
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="price"></param>
        /// <param name="vacancies"></param>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <param name="address"></param>
        /// <param name="date"></param>
        /// <param name="isCreation"></param>
        /// <returns>Returns a List of Strings (errors) or a empty List based 
        /// of the validation of the parameters </returns>
        public List<string> ValidateOpportunityParameters(
           string? name,
           string? description,
           decimal? price,
           int? vacancies,
           Category? category,
           Location? location,
           string? address,
           DateTime? date,
           bool isCreation // Indicate if this validation is for creating the Opp
)
        {
            var errors = new List<string>();

            if (isCreation)
            {
                if (string.IsNullOrWhiteSpace(name))
                    errors.Add("Name cannot be empty.");
                else if (name.Length > 100)
                    errors.Add("Name should be 100 characters or less.");

                if (string.IsNullOrWhiteSpace(description))
                    errors.Add("Description cannot be empty.");
                else if (description.Length > 1000)
                    errors.Add("Description should be 1000 characters or less.");
            }

            if (price.HasValue && price <= 0.00M)
                errors.Add("Price should be at least 0.01.");

            if (vacancies.HasValue && vacancies <= 0)
                errors.Add("Vacancies should be at least one.");

            if (category.HasValue && !Enum.IsDefined(typeof(Category), category.Value))
                errors.Add("Category is not valid.");

            if (location.HasValue && !Enum.IsDefined(typeof(Location), location.Value))
                errors.Add("Location is not valid.");

            if (!string.IsNullOrWhiteSpace(address) && address.Length > 200)
                errors.Add("Address should be 200 characters or less.");

            if (date.HasValue && date <= DateTime.Today)
                errors.Add("Date must be in the future.");

            return errors;
        }
    }
}
