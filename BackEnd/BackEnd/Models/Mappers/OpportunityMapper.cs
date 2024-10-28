using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Models.Mappers
{
    public class OpportunityMapper
    {
        // Method to map OpportunityModel to Opportunity(Dto)
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
                userId = opportunityModel.userID,
                reviewScore = opportunityModel.Score,
                date = opportunityModel.date,
                isImpulsed = opportunityModel.IsImpulsed

            };
        }

        // Method to map Opportunity(Dto) to OpportunityModel
        public static OpportunityModel MapToModel(Opportunity opportunity)
        {
            if (opportunity == null)
            {
                return null;
            }

            return new OpportunityModel
            {
                OpportunityId = opportunity.opportunityId,
                Name = opportunity.name,
                Price = opportunity.price,
                Vacancies = opportunity.vacancies,
                IsActive = opportunity.isActive,
                Category = opportunity.category,
                Description = opportunity.description,
                Location = opportunity.location,
                Address = opportunity.address,
                userID = opportunity.userId,
                Score = opportunity.reviewScore,
                date = opportunity.date,
                IsImpulsed = opportunity.isImpulsed
            };
        }
    }
}
