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

        // GET: api/User/{id}

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserByID(int id)
        {
            var response = await _userService.GetUserByIDAsync(id);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response.Data);
        }

        // POST: api/User
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

        //DELETE: api/User/2

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


        //PUT: api/User/1
        
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

        // POST: api/user/add-favorite
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

        // GET: api/user/{userId}/{opportunityId}
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

        // GET: api/user/{id}/favorites
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

        // POST: api/user/impulse
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

        // GET: api/User/created-opportunities/{id}
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

        [HttpPost("auth/google")]
        public async Task<IActionResult> GoogleLogin([FromBody] string googleToken)
        {
            //Irá ser implementado quando tivermos front-end

            throw new NotImplementedException();
        }



    }
}
