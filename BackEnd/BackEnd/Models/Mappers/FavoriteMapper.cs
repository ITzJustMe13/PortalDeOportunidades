using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Models.Mappers
{
    public class FavoriteMapper
    {
        // Method to map FavoritesModel to Favorite(Dto)
        public static Favorite MapToDto(FavoritesModel favoriteModel)
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
        public static FavoritesModel MapToModel(Favorite favorite)
        {
            if (favorite == null)
            {
                return null;
            }
            return new FavoritesModel
            {
                UserId = favorite.userId,
                OpportunityId = favorite.opportunityId
            };
        }

        internal static object MapToModel<T>(T user)
        {
            throw new NotImplementedException();
        }
    }
}