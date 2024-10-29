using BackEnd.Controllers.Data;
using BackEnd.Models;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ApplicationDbContext dbContext;
        public UserController(ApplicationDbContext dbContext) => this.dbContext = dbContext;

        // GET: api/users/2
        [HttpGet("{id}")]
        public ActionResult<User> GetUserByID(int id)
        {
            if (dbContext.Users == null)
                return NotFound();

            var user = dbContext.Users.SingleOrDefault(u => u.UserId == id);

            if (user == null)
                return NotFound();

            var u = UserMapper.MapToDto(user);

            return Ok(u);
        }

        // POST: api/users
        [HttpPost]
        public ActionResult<User> CreateNewUser(User user)
        {

            if (user.birthDate == default)
            {
                return BadRequest("O campo 'birthDate' é obrigatório.");
            }

            // validar se o user tem pelo menos 18 anos
            if ((DateTime.Now.Year - user.birthDate.Year) < 18)
            {
                return BadRequest("O utilizador deve ter pelo menos 18 anos");
            }

            try
            {
                var u = UserMapper.MapToModel(user);
                dbContext.Users.Add(u);
                dbContext.SaveChanges();

                user = UserMapper.MapToDto(u);

                return CreatedAtAction(nameof(CreateNewUser), new { user.userId }, user);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);

            }
        }

        //DELETE: api/users/2
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            if (dbContext == null)
                return NotFound();

            var user = dbContext.Users.SingleOrDefault(s => s.UserId == id);

            if (user == null)
                return NotFound();

            dbContext.Users.Remove(user);
            dbContext.SaveChanges();
            return NoContent();
        }

        // POST: api/favorites
        [HttpPost("/add-favorite")]
        public ActionResult<Favorite> AddFavorite(Favorite favorite)
        {
            // business rules validation
            // (...)

            var f = FavoriteMapper.MapToModel(favorite);

            dbContext.Favorites.Add(f);
            dbContext.SaveChanges();
            return CreatedAtAction(nameof(AddFavorite), new { favorite.userId, favorite.opportunityId }, favorite);
        }

        // POST: api/impulses
        [HttpPost("/add-impulse")]
        public ActionResult<Impulse> AddImpulse(Impulse impulse)
        {
            // business rules validation
            // (...)

            var i = ImpulseMapper.MapToModel(impulse);

            dbContext.Impulses.Add(i);
            dbContext.SaveChanges();
            return CreatedAtAction(nameof(AddImpulse), new { impulse.userId, impulse.opportunityId }, impulse);
        }
    }
}
