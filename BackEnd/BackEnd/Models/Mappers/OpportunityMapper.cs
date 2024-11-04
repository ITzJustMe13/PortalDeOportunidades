using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;


namespace BackEnd.Models.Mappers
{
    public class OpportunityMapper
    {
        // Method to map OpportunityModel to Opportunity(Dto)
        public static Opportunity MapToDto(OpportunityModel opportunityModel)
        {
            if (opportunityModel == null)
                return null;

            ValidateModel(opportunityModel);

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
                isImpulsed = opportunityModel.IsImpulsed,

               OpportunityImgs = opportunityModel.OpportunityImgs?
                    .Select(OpportunityImgMapper.MapToDto)
                    .ToList()

            };
        }

        // Method to map Opportunity(Dto) to OpportunityModel
        public static OpportunityModel MapToModel(Opportunity opportunity)
        {
            if (opportunity == null)
            {
                return null;
            }

            var opportunityModel = new OpportunityModel
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
                IsImpulsed = opportunity.isImpulsed,
                OpportunityImgs = opportunity.OpportunityImgs?
                    .Select(OpportunityImgMapper.MapToModel)
                    .ToList()

            };
             ValidateModel(opportunityModel);
            return opportunityModel;
        }

        // Método para validar o modelo através das DataAnnotations
        private static void ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, validateAllProperties: true))
            {
                // Se houver erros, lança uma exceção com detalhes
                var errorMessages = results.Select(r => r.ErrorMessage);
                throw new ValidationException("Erros de validação: " + string.Join("; ", errorMessages));
            }
        }
    }
}
