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

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserByID(int id)
        {
            if (dbContext.Users == null)
                return NotFound();

            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            var u = UserMapper.MapToDto(user);

            return Ok(u);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateNewUser(User user)
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
                await dbContext.Users.AddAsync(u);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);

            }
               
            await dbContext.SaveChangesAsync();

            

            return CreatedAtAction(nameof(GetUserByID), new { id = user.userId }, user);
        }

        //DELETE: api/users/2
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            if (dbContext == null)
                return NotFound();

            var user = await dbContext.Users.FindAsync(id);
            var reservas = await dbContext.Reservations.Where(r => r.userID == id).ToListAsync();
            var oportunidades = await dbContext.Opportunities.Where(o => o.userID == id).ToListAsync();
            var impulses = await dbContext.Impulses.Where(i => i.UserId == id).ToListAsync();

            if (user == null)
                return NotFound();

            dbContext.Users.Remove(user);
            dbContext.Reservations.RemoveRange(reservas);
            dbContext.Opportunities.RemoveRange(oportunidades);
            dbContext.Impulses.RemoveRange(impulses);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        //PUT: api/user/1/Edit?cellphoneNumber=919199233&firstName=Antonio&LastName=Carvalho  POR ALTERAR
        [HttpPut("{id}/Edit")]
        public async Task<ActionResult<User>> PutUser(int id,int? cellPhoneNumber, string? email)
        {
            if (dbContext == null)
                return NotFound();

            var user = dbContext.Users.SingleOrDefault(s => s.UserId == id);

            if (user == null)
                return NotFound();



            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/user/add-favorite
        [HttpPost("/add-favorite")]
        public async Task<ActionResult<Favorite>> AddFavorite(Favorite favorite)
        {

            if(favorite.userId == null || favorite.opportunityId == null)
            {
                return BadRequest("Invalid user or opportunity ID");
            }

            var existingFavorite = await dbContext.Favorites.AnyAsync(f => f.UserId == favorite.userId && f.OpportunityId == favorite.opportunityId);
            if (existingFavorite)
            {
                return Conflict("Favorite already exists for this user and opportunity");
            }

            var f = FavoriteMapper.MapToModel(favorite);

            await dbContext.Favorites.AddAsync(f);
            await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFavoriteById), new { favorite.userId ,favorite.opportunityId}, favorite);
        }

        // GET: api/user/{userId}/{opportunityId}
        [HttpGet("{userId}/{opportunityId}")]
        public async Task<ActionResult<Favorite>> GetFavoriteById(int userId, int opportunityId)
        {
            if (userId == null || opportunityId == null)
            {
                return BadRequest("Invalid user or opportunity ID");
            }

            var favorite = await dbContext.Favorites.FindAsync(userId,opportunityId);

            if(favorite == null)
            {
                return NotFound();
            }

            return Ok(favorite);
        }

        // GET: api/user/{id}/favorites
        [HttpGet("/{userId}/favorites")]
        public async Task<ActionResult<Favorite[]>> GetFavorites(int userId)
        {

            var favorites = await dbContext.Favorites.Where(f => f.UserId == userId).ToListAsync();

            var favoriteDTOs = favorites.Select(f => new Favorite
            {
                userId = f.UserId,
                opportunityId = f.OpportunityId,
            }).ToArray();

            if(favoriteDTOs.Length == 0)
            {
                return NotFound("No favorites found!");
            }

            return Ok(favoriteDTOs);
        }

        // POST: api/user/add-impulse
        [HttpPost("/add-impulse")]
        public ActionResult<Impulse> AddImpulse(Impulse impulse)
        {

            var i = ImpulseMapper.MapToModel(impulse);

            dbContext.Impulses.Add(i);
            dbContext.SaveChanges();
            return CreatedAtAction(nameof(AddImpulse), new { impulse.userId, impulse.opportunityId }, impulse);
        }
    }
}
