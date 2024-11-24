using BackEnd.GenericClasses;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Interfaces
{
    public interface IFavoritesService
    {

        Task<ServiceResponse<Favorite>> AddFavoriteAsync(Favorite favorite);

        Task<ServiceResponse<Favorite>> GetFavoriteByIdAsync(int userId, int opportunityId);

        Task<ServiceResponse<Favorite[]>> GetFavoritesAsync(int userId);

        Task<ServiceResponse<Favorite[]>> GetCreatedOpportunitiesAsync(int userId);
    }
}
