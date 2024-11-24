using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Mappers
{
    public class FavoriteMapper
    {
        // Method to map FavoritesModel to Favorite(Dto)
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

        // Method to map Favorite(Dto) to FavoritesModel
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