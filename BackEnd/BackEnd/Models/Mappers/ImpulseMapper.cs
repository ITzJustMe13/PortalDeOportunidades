using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Mappers
{
    public class ImpulseMapper
    {
        // Method to map ImpulseModel to Impulse(Dto)
        public static Impulse? MapToDto(ImpulseModel favoriteModel)
        {
            if (favoriteModel == null)
            {
                return null;
            }
            ValidateModel(favoriteModel);

            return new Impulse
            {
                userId = favoriteModel.UserId,
                opportunityId = favoriteModel.OpportunityId,
                value = (float)favoriteModel.Price,
                expireDate = favoriteModel.ExpireDate
            };
        }

        // Method to map Impulse(Dto) to ImpulseModel
        public static ImpulseModel? MapToModel(Impulse impulse)
        {
            if (impulse == null)
            {
                return null;
            }
            var impulseModel = new ImpulseModel
            {
                UserId = favorite.userId,
                OpportunityId = favorite.opportunityId,
                Price = (decimal)favorite.value,
                ExpireDate = favorite.expireDate
            };

            ValidateModel(impulseModel);
            return impulseModel;
        }

        // Método para validar o modelo usando DataAnnotations
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