using BackEnd.Controllers.Data;
using BackEnd.GenericClasses;
using BackEnd.Interfaces;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    /// <summary>
    /// This class is responsible for the Favorite logic of the program
    /// and implements the IFavoriteService interface
    /// Has a constructor that receives a DBContext
    /// </summary>
    public class FavoritesService : IFavoritesService
    {
        private readonly ApplicationDbContext dbContext;

        public FavoritesService(
            ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        /// <summary>
        /// Function responsible to add a Favorite to the DB
        /// </summary>
        /// <param name="favorite"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true, the Favorite Dto and a sucesseful mensage</returns>
        public async Task<ServiceResponse<Favorite>> AddFavoriteAsync(Favorite favorite)
        {
            var response = new ServiceResponse<Favorite>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (favorite.userId <= 0 || favorite.opportunityId <= 0)
                {
                    response.Success = false;
                    response.Message = "Invalid user or opportunity ID.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Verifica se já existe um favorito para o mesmo usuário e oportunidade
                var existingFavorite = await dbContext.Favorites.AnyAsync(f => f.UserId == favorite.userId && f.OpportunityId == favorite.opportunityId);
                if (existingFavorite)
                {
                    response.Success = false;
                    response.Message = "Favorite already exists for this user and opportunity.";
                    response.Type = "Conflict";
                    return response;
                }

                // Mapeia o DTO para o modelo
                var favoriteModel = FavoriteMapper.MapToModel(favorite);

                // Adiciona o favorito à base de dados
                await dbContext.Favorites.AddAsync(favoriteModel);
                await dbContext.SaveChangesAsync();

                // Mapeia o modelo para DTO
                var favoriteDTO = FavoriteMapper.MapToDto(favoriteModel);

                response.Data = favoriteDTO;
                response.Success = true;
                response.Message = "Favorite added successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while adding the favorite.";
                response.Type = "BadRequest";
            }

            return response;
        }

        /// <summary>
        /// Function that gets the Favorite by its id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="opportunityId"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true,the Favorite Dto and a sucesseful mensage</returns>
        public async Task<ServiceResponse<Favorite>> GetFavoriteByIdAsync(int userId, int opportunityId)
        {
            var response = new ServiceResponse<Favorite>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (userId <= 0 || opportunityId <= 0)
                {
                    response.Success = false;
                    response.Message = "Invalid user or opportunity ID.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Busca o favorito no banco de dados
                var favorite = await dbContext.Favorites
                .Where(f => f.UserId == userId && f.OpportunityId == opportunityId)
                .FirstOrDefaultAsync();

                if (favorite == null)
                {
                    response.Success = false;
                    response.Message = "Favorite not found.";
                    response.Type = "NotFound";
                    return response;
                }

                // Mapeia o modelo para o DTO
                var favoriteDTO = FavoriteMapper.MapToDto(favorite);

                response.Data = favoriteDTO;
                response.Success = true;
                response.Message = "Favorite retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while retrieving the favorite.";
                response.Type = "BadRequest";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }

        /// <summary>
        /// Function that gets all the User Favorites by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true,the array of Favorite Dto 
        /// and a sucesseful mensage</returns>
        public async Task<ServiceResponse<Favorite[]>> GetFavoritesAsync(int userId)
        {
            var response = new ServiceResponse<Favorite[]>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (userId <= 0)
                {
                    response.Success = false;
                    response.Message = "Invalid userId.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Busca os favoritos do utilizador
                var favorites = await dbContext.Favorites
                    .Where(f => f.UserId == userId)
                    .ToListAsync();

                if (!favorites.Any())
                {
                    response.Success = false;
                    response.Message = "No favorites found!";
                    response.Type = "NotFound";
                    return response;
                }

                var favoriteDTOs = favorites.Select(f => new Favorite
                {
                    userId = f.UserId,
                    opportunityId = f.OpportunityId,
                }).ToArray();

                // Mapeia os modelos para DTOs
                response.Data = favoriteDTOs;
                response.Success = true;
                response.Message = "Favorites retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while retrieving favorites.";
                response.Type = "BadRequest";
            }

            return response;
        }

        /// <summary>
        /// Function that gets all the created Opportunities of a User by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true,the array of Opportunity Dto 
        /// and a sucesseful mensage</returns>
        public async Task<ServiceResponse<Favorite[]>> GetCreatedOpportunitiesAsync(int userId)
        {
            var response = new ServiceResponse<Favorite[]>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (userId <= 0)
                {
                    response.Success = false;
                    response.Message = "Invalid userId.";
                    response.Type = "BadRequest";
                    return response;
                }

                var opportunities = await dbContext.Opportunities.Where(o => o.UserID == userId).ToListAsync();

                if (!opportunities.Any())
                {
                    response.Success = false;
                    response.Message = "You have no created opportunities!";
                    response.Type = "NotFound";
                    return response;
                }

                var opportunitiesDTOs = opportunities.Select(o => new Favorite
                {
                    userId = o.UserID,
                    opportunityId = o.OpportunityId
                }).ToArray();

                response.Data = opportunitiesDTOs;
                response.Success = true;
                response.Message = "Opportunities retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while fetching opportunities.";
                response.Type = "BadRequest";
            }

            return response;
        }
    }
}
