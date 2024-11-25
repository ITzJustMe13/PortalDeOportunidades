using BackEnd.Enums;
using BackEnd.ServiceResponses;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Interfaces
{
    public interface IOpportunityService
    {
        Task<ServiceResponse<IEnumerable<Opportunity>>> GetAllOpportunitiesAsync();

        Task<ServiceResponse<IEnumerable<Opportunity>>> GetAllImpulsedOpportunitiesAsync();

        Task<ServiceResponse<Opportunity>> GetOpportunityByIdAsync(int id);

        Task<ServiceResponse<List<Opportunity>>> GetAllOpportunitiesByUserIdAsync(int userId);

        Task<ServiceResponse<List<Opportunity>>> SearchOpportunitiesAsync(
            string? keyword,
            int? vacancies,
            decimal? minPrice,
            decimal? maxPrice,
            Category? category,
            Location? location
        );

        Task<ServiceResponse<Opportunity>> CreateOpportunityAsync(Opportunity opportunityDto);

        Task<ServiceResponse<bool>> DeleteOpportunityByIdAsync(int id);

        Task<ServiceResponse<bool>> ActivateOpportunityByIdAsync(int id);

        Task<ServiceResponse<bool>> DeactivateOpportunityByIdAsync(int id);

        Task<ServiceResponse<Opportunity>> EditOpportunityByIdAsync(
            int id,
            string? name,
            string? description,
            decimal? price,
            int? vacancies,
            Category? category,
            Location? location,
            string? address,
            DateTime? date,
            List<byte[]>? newImageUrls
        );
    }
}
