using BackEnd.Enums;

namespace BackEnd.Interfaces
{
    public interface IOpportunityService
    {
        List<string> ValidateSearchParameters(int? vacancies, decimal? minPrice, decimal? maxPrice, Category? category, Location? location);

        List<string> ValidateOpportunityParameters(
           string? name,
           string? description,
           decimal? price,
           int? vacancies,
           Category? category,
           Location? location,
           string? address,
           DateTime? date,
           bool isCreation);
    }
}
