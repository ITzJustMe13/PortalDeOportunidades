using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Mappers
{
    public class OpportunityImgMapper
    {
        // Method to map OpportunityImgModel to OpportunityImg(Dto)
        public static OpportunityImg MapToDto(OpportunityImgModel opportunityImgModel)
        {
            if (opportunityImgModel == null)
                return null;

            ValidateModel(opportunityImgModel);

            return new OpportunityImg
            {
                ImgId = opportunityImgModel.ImgId,
                OpportunityId = opportunityImgModel.OpportunityId,
                Image = opportunityImgModel.Image

            };
        }

        // Method to map OpportunityImg(Dto) to OpportunityImgModel
        public static OpportunityImgModel MapToModel(OpportunityImg opportunityImg)
        {
            if (opportunityImg == null)
            {
                return null;
            }

            var opportunityImgModel = new OpportunityImgModel
            {
                ImgId = opportunityImg.ImgId,
                OpportunityId = opportunityImg.OpportunityId,
                Image = opportunityImg.Image
            };
            ValidateModel(opportunityImgModel);
            return opportunityImgModel;
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

