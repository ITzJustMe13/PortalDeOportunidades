using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;


namespace BackEnd.Models.Mappers
{
    /// <summary>
    /// class that maps OpportunityModel to Dto and vice-versa
    /// </summary>
    public class OpportunityMapper
    {
        /// <summary>
        /// Function that maps OpportunityModel parameters to Opportunity parameters
        /// </summary>
        /// <param name="opportunityModel"></param>
        /// <returns>Returns a Dto with the Model info</returns>
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
                userId = opportunityModel.UserID,
                reviewScore = opportunityModel.Score,
                date = opportunityModel.Date,
                isImpulsed = opportunityModel.IsImpulsed,

               OpportunityImgs = opportunityModel.OpportunityImgs?
                    .Select(OpportunityImgMapper.MapToDto)
                    .ToList()

            };
        }

        /// <summary>
        /// Function that maps Opportunity parameters to OpportunityModel parameters
        /// </summary>
        /// <param name="opportunity"></param>
        /// <returns>Returns a Model with the Dto info</returns>
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
                UserID = opportunity.userId,
                Score = opportunity.reviewScore,
                Date = opportunity.date,
                IsImpulsed = opportunity.isImpulsed,
                OpportunityImgs = opportunity.OpportunityImgs?
                    .Select(OpportunityImgMapper.MapToModel)
                    .ToList()

            };
             ValidateModel(opportunityModel);
            return opportunityModel;
        }

        /// <summary>
        /// Function that validates if the mapping parameters are correct with each other
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="ValidationException"></exception>
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
