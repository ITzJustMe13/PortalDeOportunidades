using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Models.Mappers
{
    public class OpportunityMapper
    {
        // Method to map OpportunityModel to Opportunity
        public static Opportunity MapToDto(OpportunityModel opportunityModel)
        {
            if (opportunityModel == null)
                return null;

            return new Opportunity
            {
                opportunityId = opportunityModel.OpportunityId,
                name = opportunityModel.Name,
                price = opportunityModel.Price,
                vacancies = opportunityModel.Vacancies,
                isActive = opportunityModel.IsActive,
                category = opportunityModel.Category,
                description = opportunityModel.Description,
                location = opportunityModel.Location,
                address = opportunityModel.Address,
                
            };
        }
    }
}
