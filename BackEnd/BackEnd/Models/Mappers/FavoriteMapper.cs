using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Mappers
{
    public class FavoriteMapper
    {
        /// <summary>
        /// Function that maps FavoritesModel parameters to Favorite parameters
        /// </summary>
        /// <param name="favoriteModel"></param>
        /// <returns>Returns a Dto with the Model info</returns>
        public static Favorite? MapToDto(FavoritesModel favoriteModel)
        {
            if (favoriteModel == null)
            {
                return null;
            }
            return new Favorite
            {
                userId = favoriteModel.UserId,
                opportunityId=favoriteModel.OpportunityId

            };
        }

        /// <summary>
        /// Function that maps Favorites parameters to FavoriteModel parameters
        /// </summary>
        /// <param name="favorite"></param>
        /// <returns>Returns a Model with the Dto info</returns>
        public static FavoritesModel? MapToModel(Favorite favorite)
        {
            if (favorite == null)
            {
                return null;
            }
            var favoriteModel = new FavoritesModel
            {
                UserId = favorite.userId,
                OpportunityId = favorite.opportunityId
            };

            ValidateModel(favoriteModel);
            return favoriteModel;
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