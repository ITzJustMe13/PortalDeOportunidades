using BackEnd.Controllers.Data;
using BackEnd.Models;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using BackEnd.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        [Authorize]
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
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // Return the validation errors
                return BadRequest(ModelState);
            }

            if (user.birthDate == default)
            {
                return BadRequest("O campo 'birthDate' é obrigatório.");
            }

            // validar se o user tem pelo menos 18 anos
            if ((DateTime.Now.Year - user.birthDate?.Year) < 18)
            {
                return BadRequest("O utilizador deve ter pelo menos 18 anos");
            }

            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(user.password, 13);

            try
            {
                var u = UserMapper.MapToModel(user);
                if (u == null)
                    return Problem("Erro ao mapear o Model do user");

                u.HashedPassword = passwordHash;

                await dbContext.Users.AddAsync(u);
                await dbContext.SaveChangesAsync();
                var createdUserDTO = UserMapper.MapToDto(u);

                if (createdUserDTO == null)
                    return Problem("Erro na criação do DTO");


                return CreatedAtAction(nameof(GetUserByID), new { createdUserDTO.userId }, createdUserDTO);

            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);

            }
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
        public async Task<ActionResult<User>> PutUser(int id, int? cellPhoneNumber, string? email)
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

            if (favorite.userId == 0 || favorite.opportunityId == 0)
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
            return CreatedAtAction(nameof(GetFavoriteById), new { favorite.userId, favorite.opportunityId }, favorite);
        }

        // GET: api/user/{userId}/{opportunityId}
        [HttpGet("{userId}/{opportunityId}")]
        public async Task<ActionResult<Favorite>> GetFavoriteById(int userId, int opportunityId)
        {
            if (userId == 0 || opportunityId == 0)
            {
                return BadRequest("Invalid user or opportunity ID");
            }

            var favorite = await dbContext.Favorites.FindAsync(userId, opportunityId);

            if (favorite == null)
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

            if (favoriteDTOs.Length == 0)
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

        [HttpPost("/login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return BadRequest(new { message = "Username or password is incorrect" });

            var user = await dbContext.Users.FirstOrDefaultAsync(f => f.Email == email);

            if (user == null)
                return NotFound("Nenhum utilizador encontrado");

            if (!BCrypt.Net.BCrypt.EnhancedVerify(password, user.HashedPassword))
            {
                return Unauthorized("A autenticação falhou, por favor verifique as suas informações.");
            }

            try
            {
                var authenticatedUserDTO = UserMapper.MapToDto(user);

                if (authenticatedUserDTO == null)
                    return Problem("Erro ao mapear o UserDTO");

                var token = AuthService.GenerateToken(authenticatedUserDTO);

                return Ok(new { token, authenticatedUserDTO });

            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }
    }
}
