using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Mappers
{
    public class OpportunityImgMapper
    {
        /// <summary>
        /// Function that maps OpportunityImgModel parameters to OpportunityImg parameters
        /// </summary>
        /// <param name="opportunityImgModel"></param>
        /// <returns>Returns a Dto with the Model info</returns>
        public static OpportunityImg MapToDto(OpportunityImgModel opportunityImgModel)
        {
            if (opportunityImgModel == null)
                return null;

            ValidateModel(opportunityImgModel);

            return new OpportunityImg
            {
                imgId = opportunityImgModel.ImgId,
                opportunityId = opportunityImgModel.OpportunityId,
                image = opportunityImgModel.Image

            };
        }

        /// <summary>
        /// Function that maps OpportunityImg parameters to OpportunityImgModel parameters
        /// </summary>
        /// <param name="opportunityImg"></param>
        /// <returns>Returns a Model with the Dto info</returns>
        public static OpportunityImgModel MapToModel(OpportunityImg opportunityImg)
        {
            if (opportunityImg == null)
            {
                return null;
            }

            var opportunityImgModel = new OpportunityImgModel
            {
                ImgId = opportunityImg.imgId,
                OpportunityId = opportunityImg.opportunityId,
                Image = opportunityImg.image
            };
            ValidateModel(opportunityImgModel);
            return opportunityImgModel;
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

