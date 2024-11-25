using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BackEnd.Services;
using BackEnd.Interfaces;
using BackEnd.GenericClasses;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFavoritesService _favoritesService;
        public UserController(IUserService userService, IFavoritesService favoritesService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _favoritesService = favoritesService ?? throw new ArgumentNullException(nameof(favoritesService));
        }

        /// <summary>
        /// Endpoint that gets the user by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserByID(int id)
        {
            var response = await _userService.GetUserByIDAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }

        /// <summary>
        /// Endpoint that creates a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateNewUser(User user)
        {
            var response = await _userService.CreateNewUserAsync(user);

            if (!response.Success && response.Type.Equals("BadRequest"))
            {
                return BadRequest(response.Message);
            }

            if (!response.Success && response.Type.Equals("NotFound"))
            {
                return NotFound(response.Message);
            }

         return CreatedAtAction(nameof(GetUserByID), new { id = response.Data.userId }, response.Data);
        }

        /// <summary>
        /// Endpoint that deletes a user by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            {
                var serviceResponse = await _userService.DeleteUserAsync(id);

                if (!serviceResponse.Success && serviceResponse.Type.Equals("BadRequest"))
                {
                    return BadRequest(serviceResponse.Message);
                }

                if (!serviceResponse.Success && serviceResponse.Type.Equals("NotFound"))
                {
                    return NotFound(serviceResponse.Message);
                }

             return NoContent();
            }
        }

        
        /// <summary>
        /// Endpoint that edits a user by his id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedUser"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditUser(int id, User updatedUser)
        {
            var serviceResponse = await _userService.EditUserAsync(id, updatedUser);

            if (!serviceResponse.Success && serviceResponse.Type.Equals("BadRequest"))
            {
                return BadRequest(serviceResponse.Message);
            }

            if (!serviceResponse.Success && serviceResponse.Type.Equals("NotFound"))
            {
                return NotFound(serviceResponse.Message);
            }

            return Ok(serviceResponse.Data);
        }

        /// <summary>
        /// Endpoint that adds a Favorite to a user
        /// </summary>
        /// <param name="favorite"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("favorite")]
        public async Task<IActionResult> AddFavorite(Favorite favorite)
        {
            var serviceResponse = await _favoritesService.AddFavoriteAsync(favorite);

            if (!serviceResponse.Success && serviceResponse.Type.Equals("BadRequest"))
            {
                return BadRequest(serviceResponse.Message);
            }

            if (!serviceResponse.Success && serviceResponse.Type.Equals("NotFound"))
            {
                return NotFound(serviceResponse.Message);
            }

            if (!serviceResponse.Success && serviceResponse.Type.Equals("Conflict"))
            {
                return Conflict(serviceResponse.Message);
            }

            return CreatedAtAction(nameof(GetFavoriteById), new { userId = favorite.userId, opportunityId = favorite.opportunityId }, serviceResponse.Data);
        }

        /// <summary>
        /// Endpoint that gets a User Favorite by its id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="opportunityId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("favorite/{userId}/{opportunityId}")]
        public async Task<IActionResult> GetFavoriteById(int userId, int opportunityId)
        {
            var serviceResponse = await _favoritesService.GetFavoriteByIdAsync(userId, opportunityId);

            if (!serviceResponse.Success && serviceResponse.Type.Equals("BadRequest"))
            {
              return BadRequest(serviceResponse.Message);
            }

            if (!serviceResponse.Success && serviceResponse.Type.Equals("NotFound"))
            {
                return NotFound(serviceResponse.Message);
            }

            return Ok(serviceResponse.Data);
        }

        /// <summary>
        /// Endpoint that gets all User Favorites by User id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("favorites/{userId}")]
        public async Task<IActionResult> GetFavorites(int userId)
        {
            var serviceResponse = await _favoritesService.GetFavoritesAsync(userId);

            if (!serviceResponse.Success && serviceResponse.Type.Equals("BadRequest"))
            {
              return BadRequest(serviceResponse.Message);
            }

            if (!serviceResponse.Success && serviceResponse.Type.Equals("NotFound"))
            {
                return NotFound(serviceResponse.Message);
            }


            return Ok(serviceResponse.Data);
        }

        /// <summary>
        /// Endpoint that makes an Opportunity impulsed
        /// </summary>
        /// <param name="impulse"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("impulse")]
        public async Task<IActionResult> ImpulseOportunity(Impulse impulse)
        {
            var serviceResponse = await _userService.ImpulseOpportunityAsync(impulse);

            if (!serviceResponse.Success && serviceResponse.Type.Equals("BadRequest"))
            {
                 return BadRequest(serviceResponse.Message);

            }

            if (!serviceResponse.Success && serviceResponse.Type.Equals("NotFound"))
            {
                return NotFound(serviceResponse.Message);

            }

            return CreatedAtAction(nameof(ImpulseOportunity), new
            {
                serviceResponse.Data.userId,
                serviceResponse.Data.opportunityId
            }, serviceResponse.Data);
        }

        /// <summary>
        /// Endpoint that gets all the created Opportunities of a User by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("created-opportunities/{userId}")]
        public async Task<IActionResult> GetCreatedOpportunities(int userId)
        {
            var serviceResponse = await _favoritesService.GetCreatedOpportunitiesAsync(userId);

            if (!serviceResponse.Success && serviceResponse.Type.Equals("BadRequest"))
            {
             return BadRequest(serviceResponse.Message);
            }

            if (!serviceResponse.Success && serviceResponse.Type.Equals("NotFound"))
            {
             return NotFound(serviceResponse.Message);
            }

         return Ok(serviceResponse.Data);
        }

        /// <summary>
        /// Endpoint responsible for the User login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
           var serviceResponse = await _userService.LoginAsync(request);

           if (!serviceResponse.Success && serviceResponse.Type.Equals("BadRequest"))
           {
                return BadRequest(serviceResponse.Message);
           }

            if (!serviceResponse.Success && serviceResponse.Type.Equals("NotFound"))
            {
                return NotFound(serviceResponse.Message);
            }

            if (!serviceResponse.Success && serviceResponse.Type.Equals("Unauthorized"))
            {
                return Unauthorized(serviceResponse.Message);
            }

            return Ok(serviceResponse.Data);

        }


        /// <summary>
        /// Endpoint that gets if the email is available or not
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("get-email-availability/{email}")]
        public async Task<IActionResult> GetEmailAvailability(string email)
        {
            var serviceResponse = await _userService.CheckEmailAvailabilityAsync(email);

            if (!serviceResponse.Success && serviceResponse.Type.Equals("BadRequest"))
            {
                if (serviceResponse.Errors != null && serviceResponse.Errors.Any())
                {
                    return BadRequest(serviceResponse.Message);
                }

                return BadRequest(serviceResponse.Message);
            }

            if (!serviceResponse.Success && serviceResponse.Type.Equals("NotFound"))
            {
                return NotFound(serviceResponse.Message);
            }

         return Ok(serviceResponse.Data);
        }

        /// <summary>
        /// Endpoint that activates User account
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("activate")]
        public async Task<IActionResult> ActivateAccount([FromQuery]string token)
        {
            var serviceResponse = await _userService.ActivateAccountAsync(token);

            if (!serviceResponse.Success)
            {
                if (serviceResponse.Errors != null && serviceResponse.Errors.Any())
                {
                    return BadRequest(serviceResponse.Message);
                }

                return BadRequest(serviceResponse.Message);
            }

            return Ok(serviceResponse.Data);
        }

        /// <summary>
        /// Endpoint for using google auth
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
