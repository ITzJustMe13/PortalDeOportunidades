using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Mappers
{
    /// <summary>
    /// class that maps ImpulseModel to Dto and vice-versa
    /// </summary>
    public class ImpulseMapper
    {
        /// <summary>
        /// Function that maps ImpulseModel parameters to Impulse parameters
        /// </summary>
        /// <param name="impulseModel"></param>
        /// <returns>Returns a Dto with the Model info</returns>
        public static Impulse? MapToDto(ImpulseModel impulseModel)
        {
            if (impulseModel == null)
            {
                return null;
            }

            return new Impulse
            {
                userId = impulseModel.UserId,
                opportunityId = impulseModel.OpportunityId,
                value = (float)impulseModel.Price,
                expireDate = impulseModel.ExpireDate
            };
        }

        /// <summary>
        /// Function that maps Impulse parameters to ImpulseModel parameters
        /// </summary>
        /// <param name="impulse"></param>
        /// <returns>Returns a Model with the Dto info</returns>
        public static ImpulseModel? MapToModel(Impulse impulse)
        {
            if (impulse == null)
            {
                return null;
            }
            var impulseModel = new ImpulseModel
            {
                UserId = impulse.userId,
                OpportunityId = impulse.opportunityId,
                Price = (decimal)impulse.value,
                ExpireDate = impulse.expireDate
            };

           
            ValidateModel(impulseModel);

            return impulseModel;
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