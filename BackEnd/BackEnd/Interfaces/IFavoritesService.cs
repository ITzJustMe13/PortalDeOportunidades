using BackEnd.ServiceResponses;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Interfaces
{
    /// <summary>
    /// This interface is responsibile for all the functions of the logic part of Favorites
    /// </summary>
    public interface IFavoritesService
    {
        /// <summary>
        /// Function responsible to add a Favorite to the DB
        /// </summary>
        /// <param name="favorite"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true, the Favorite Dto and a sucesseful mensage</returns>
        Task<ServiceResponse<Favorite>> AddFavoriteAsync(Favorite favorite);

        /// <summary>
        /// Function that gets the Favorite by its id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="opportunityId"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true,the Favorite Dto and a sucesseful mensage</returns>
        Task<ServiceResponse<Favorite>> GetFavoriteByIdAsync(int userId, int opportunityId);


        /// <summary>
        /// Function that gets all the User Favorites by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true,the array of Favorite Dto 
        /// and a sucesseful mensage</returns>
        Task<ServiceResponse<Favorite[]>> GetFavoritesAsync(int userId);


        /// <summary>
        /// Function that gets all the created Opportunities of a User by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true,the array of Opportunity Dto 
        /// and a sucesseful mensage</returns>
        Task<ServiceResponse<Favorite[]>> GetCreatedOpportunitiesAsync(int userId);

        /// <summary>
        /// Function that deletes a Favorite by its user id and opportunity id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oppId"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true if deleted sucessefully</returns>
        Task<ServiceResponse<bool>> DeleteFavoriteByIdAsync(int userId, int oppId);
    }
}
