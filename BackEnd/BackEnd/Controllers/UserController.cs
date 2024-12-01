using BackEnd.Models.FrontEndModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Interfaces;
using BackEnd.ServiceResponses;

namespace BackEnd.Controllers
{
    /// <summary>
    /// Controller responsible for all endpoints relating to User
    /// Has a constructor that receives an IUserService and an IFavoritesService instance
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseCrudController<User>
    {
        private readonly IUserService _userService;
        private readonly IFavoritesService _favoritesService;
        public UserController(IUserService userService, IFavoritesService favoritesService, IConfiguration configuration) : base(configuration)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _favoritesService = favoritesService ?? throw new ArgumentNullException(nameof(favoritesService));
        }

        
        /// <summary>
        /// Endpoint that gets a certain User by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns NotFound() if is not sucessefully or OK() with the User Dto if it is</returns>
        [HttpGet("{id}")]
        [Authorize]
        public override async Task<IActionResult> GetEntityById(int id)
        {
            var serviceResponse = await _userService.GetUserByIDAsync(id);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that creates a new User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns BadRequest() if userService responds "BadRequest", 
        /// NotFound() if userService responds "NotFound", or CreatedAtAction()
        /// with the path for the newly created user</returns>
        [HttpPost]
        public override async Task<IActionResult> CreateEntity(User user)
        {
            var serviceResponse = await _userService.CreateNewUserAsync(user);

            if (!serviceResponse.Success)
            {
                return HandleResponse(serviceResponse);
            }

            return HandleCreatedAtAction(serviceResponse, nameof(GetEntityById), new { id = serviceResponse.Data.userId });
        }

        /// <summary>
        /// Endpoint that deletes the User by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns BadRequest() if userService responds "BadRequest", 
        /// NotFound() if userService responds "NotFound", or NoContent() if
        /// user is correctly deleted from the system</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public override async Task<IActionResult> DeleteEntity(int id)
        {
            {
                var serviceResponse = await _userService.DeleteUserAsync(id);

                return HandleResponse(serviceResponse);
            }
        }

        /// <summary>
        /// Endpoint that edits the user based on an updated dto that it receives by his id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedUser"></param>
        /// <returns>Returns BadRequest() if userService responds "BadRequest", 
        /// NotFound() if userService responds "NotFound", or Ok() with
        /// updated User dto if updated correctly</returns>
        [HttpPut("{id}")]
        [Authorize]
        public override async Task<IActionResult> UpdateEntity(int id, User updatedUser)
        {
            var serviceResponse = await _userService.EditUserAsync(id, updatedUser);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that adds a Favorite Opportunity to Users favorites
        /// </summary>
        /// <param name="favorite"></param>
        /// <returns>Returns BadRequest() if userService responds "BadRequest", 
        /// NotFound() if userService responds "NotFound", Conflict() if
        /// userService responds "Conflict" or CreatedAtAction() if
        /// the Favorite is correctly created</returns>
        [Authorize]
        [HttpPost("favorite")]
        public async Task<IActionResult> AddFavorite(Favorite favorite)
        {
            var serviceResponse = await _favoritesService.AddFavoriteAsync(favorite);

            if (!serviceResponse.Success)
            {
                return HandleResponse(serviceResponse);
            }

            return HandleCreatedAtAction(serviceResponse, nameof(GetFavoriteById), new { userId = favorite.userId, opportunityId = favorite.opportunityId });
        }

        /// <summary>
        /// Endpoint that gets a Favorite by his composed id (userId + opportunityId)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="opportunityId"></param>
        /// <returns>Returns BadRequest() if userService responds "BadRequest", 
        /// NotFound() if userService responds "NotFound", or Ok() with
        /// the Favorite dto</returns>
        [Authorize]
        [HttpGet("favorite/{userId}/{opportunityId}")]
        public async Task<IActionResult> GetFavoriteById(int userId, int opportunityId)
        {
            var serviceResponse = await _favoritesService.GetFavoriteByIdAsync(userId, opportunityId);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that gets all favorites of a User by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Returns BadRequest() if userService responds "BadRequest", 
        /// NotFound() if userService responds "NotFound", or Ok() with
        /// all the user's favorites</returns>
        [Authorize]
        [HttpGet("favorites/{userId}")]
        public async Task<IActionResult> GetFavorites(int userId)
        {
            var serviceResponse = await _favoritesService.GetFavoritesAsync(userId);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that adds an Impulse to a User's Opportunity
        /// </summary>
        /// <param name="impulse"></param>
        /// <returns>Returns BadRequest() if userService responds "BadRequest", 
        /// NotFound() if userService responds "NotFound", or CreatedAtAction()
        /// if the Impulse is created correctly</returns>
        [Authorize]
        [HttpPost("impulse")]
        public async Task<IActionResult> ImpulseOportunity(Impulse impulse)
        {
            var serviceResponse = await _userService.ImpulseOpportunityAsync(impulse);

            if (!serviceResponse.Success)
            {
                return HandleResponse(serviceResponse);
            }

            return HandleCreatedAtAction(serviceResponse, nameof(ImpulseOportunity), new
            {
                serviceResponse.Data.userId,
                serviceResponse.Data.opportunityId
            });
        }

        /// <summary>
        /// Endpoint that gets all the Opportunities created by a User by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Returns BadRequest() if userService responds "BadRequest", 
        /// NotFound() if userService responds "NotFound", or Ok() with
        /// all the Opportunities created by the User</returns>
        [Authorize]
        [HttpGet("created-opportunities/{userId}")]
        public async Task<IActionResult> GetCreatedOpportunities(int userId)
        {
            var serviceResponse = await _favoritesService.GetCreatedOpportunitiesAsync(userId);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that makes the Login of the User
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns BadRequest() if userService responds "BadRequest", 
        /// NotFound() if userService responds "NotFound", Unauthorized if
        /// userService responds with "Unauthorized or Ok() with the
        /// User's Jwt Token and Info</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
           var serviceResponse = await _userService.LoginAsync(request);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that informs the user if his email is already registered in the plataform
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Returns BadRequest() if userService responds "BadRequest", 
        /// NotFound() if userService responds "NotFound", or Ok() with
        /// a message according with if the user's email is already registered or not</returns>
        [HttpGet("get-email-availability/{email}")]
        public async Task<IActionResult> GetEmailAvailability(string email)
        {
            var serviceResponse = await _userService.CheckEmailAvailabilityAsync(email);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that activates User's account on the DB and plataform
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Returns BadRequest() if userService responds "BadRequest", 
        /// or Ok() with a sucesseful mensage</returns>
        [HttpGet("activate")]
        public async Task<IActionResult> ActivateAccount([FromQuery]string token)
        {
            var serviceResponse = await _userService.ActivateAccountAsync(token);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that lets the User use Google Auth to autenticate in the plataform
        /// </summary>
        /// <param name="googleToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost("auth/google")]
        public async Task<IActionResult> GoogleLogin([FromBody] string googleToken)
        {
            //Irá ser implementado quando tivermos front-end

            throw new NotImplementedException();
        }

    }
}
