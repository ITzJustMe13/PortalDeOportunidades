using BackEnd.Enums;

namespace BackEnd.Services
{
    public class OpportunityService
    {
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
