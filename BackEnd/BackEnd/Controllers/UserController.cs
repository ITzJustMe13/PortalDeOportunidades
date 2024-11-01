using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.Models;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using BackEnd.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ApplicationDbContext dbContext;
        public UserController(ApplicationDbContext dbContext) => this.dbContext = dbContext;

        // GET: api/User/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserByID(int id)
        {
            if (dbContext.Users == null)
                return NotFound();

            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
                return NotFound("O utilizador não foi encontrado");

            var u = UserMapper.MapToDto(user);

            return Ok(u);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> CreateNewUser(User user)
        {
            if (dbContext == null)
            {
                return NotFound("Database context is not available.");
            }

            // Verifica se o dto é valido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await IsEmailAvailable(user.email))
            {
                return BadRequest("O 'email' já está a ser utilizado");
            }

            // validar se o gender posto existe
            if (!Enum.IsDefined(typeof(Gender), user.gender))
            {
                return BadRequest("O campo 'gender' tem um valor inválido'");
            }

            // validar se o utilizador criou conta ou fez registo por meios externos
            if (user.password == null)
            {
                return BadRequest("O campo 'password' é necessário ao efetuar registo pelo sistema'");
            }

            // validar se o utilizador tem pelo menos 18 anos
            if ((DateTime.Now.Year - user.birthDate.Year) < 18)
            {
                return BadRequest("O utilizador deve ter pelo menos 18 anos");
            }

            // encripta a password do utilizador
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(user.password, 13);

            try
            {
                var u = UserMapper.MapToModel(user);

                if (u == null)
                    return Problem("Erro ao mapear o Model do user");

                u.HashedPassword = passwordHash;
                u.isActive = false;

                var activationToken = AuthService.GenerateToken(u);
                u.Token = activationToken;
                u.TokenExpDate = DateTime.UtcNow.AddHours(24);
                await dbContext.SaveChangesAsync();
                SendActivationEmail(u);

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

        //DELETE: api/User/2
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            if (dbContext == null)
                return NotFound("O contexto da BD não foi encontrado");

            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
                return NotFound("O utilizador não foi encontrado");

            // encontra e elimina as reservas, oportunidades e impulsos do utilizador
            var reservas = await dbContext.Reservations.Where(r => r.userID == id).ToListAsync();
            var oportunidades = await dbContext.Opportunities.Where(o => o.userID == id).ToListAsync();
            var impulses = await dbContext.Impulses.Where(i => i.UserId == id).ToListAsync();

            dbContext.Users.Remove(user);
            dbContext.Reservations.RemoveRange(reservas);
            dbContext.Opportunities.RemoveRange(oportunidades);
            dbContext.Impulses.RemoveRange(impulses);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        //PUT: api/User/1
        [HttpPut("{id}")]
        public async Task<ActionResult> EditUser(int id, [FromBody] User updatedUser)
        {
            if (dbContext == null)
                return NotFound("Database context is not available.");

            // Find the existing user by ID
            var existingUser = await dbContext.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound("Utilizador não encontrado.");

            if (!await IsEmailAvailable(updatedUser.email))
            {
                return BadRequest("O 'email' já está a ser utilizado");
            }

            // validar se o gender posto existe
            if (!Enum.IsDefined(typeof(Gender), updatedUser.gender))
            {
                return BadRequest("O campo 'gender' tem um valor inválido'");
            }

            // validar se o utilizador tem pelo menos 18 anos
            if ((DateTime.Now.Year - updatedUser.birthDate.Year) < 18)
            {
                return BadRequest("O utilizador deve ter pelo menos 18 anos");
            }

            // Atualizar as informações do utilizador existente com as novas informações
            existingUser.FirstName = updatedUser.firstName;
            existingUser.LastName = updatedUser.lastName;
            existingUser.Email = updatedUser.email;
            existingUser.CellPhoneNum = updatedUser.cellPhoneNumber;
            existingUser.BirthDate = updatedUser.birthDate;
            existingUser.Gender = updatedUser.gender;

            if (!String.IsNullOrEmpty(updatedUser.password))
            {
                string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(updatedUser.password, 13);
                existingUser.HashedPassword = passwordHash;
            }

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the user: {ex.Message}");
            }

            return NoContent();
        }


        // POST: api/user/add-favorite
        [HttpPost("favorite")]
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
        [HttpGet("favorite/{userId}/{opportunityId}")]
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
        [HttpGet("favorites/{userId}")]
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
        [HttpPost("impulse")]
        public ActionResult<Impulse> ImpulseOportunity(Impulse impulse)
        {

            var i = ImpulseMapper.MapToModel(impulse);

            dbContext.Impulses.Add(i);
            dbContext.SaveChanges();
            return CreatedAtAction(nameof(ImpulseOportunity), new { impulse.userId, impulse.opportunityId }, impulse);
        }

        // GET: api/User/created-opportunities/{id}
        [HttpGet("created-opportunities/{userId}")]
        public async Task<ActionResult<Favorite[]>> GetCreatedOpportunities(int userId)
        {

            var opportunities = await dbContext.Opportunities.Where(o => o.userID == userId).ToListAsync();

            var opportunitiesDTOs = opportunities.Select(o => new Favorite
            {
                userId = o.userID,
                opportunityId = o.OpportunityId,
            }).ToArray();

            if (opportunitiesDTOs.Length == 0)
            {
                return NotFound("Nenhuma oportunidade criada encontrada!");
            }

            return Ok(opportunitiesDTOs);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username or password is incorrect");

            var user = await dbContext.Users.SingleOrDefaultAsync(f => f.Email == request.Email);

            if (user == null)
                return Unauthorized("A autenticação falhou, por favor verifique as suas informações.");

            if (!user.isActive)
            {
                return BadRequest("Account is not activated. Please check your email for the activation link.");
            }

            if (!BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.HashedPassword))
            {
                return Unauthorized("A autenticação falhou, por favor verifique as suas informações.");
            }

            try
            {
                var authenticatedUserDTO = UserMapper.MapToDto(user);

                if (authenticatedUserDTO == null)
                    return Problem("Erro ao mapear o UserDTO");

                var token = AuthService.GenerateToken(user);

                return Ok(new { token, authenticatedUserDTO });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }

        private async Task<Boolean> IsEmailAvailable(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return !await dbContext.Users.AnyAsync(user => user.Email == email);
        }

        [HttpGet("get-email-availability/{email}")]
        public async Task<IActionResult> GetEmailAvailability(string email)
        {
            var emailAvailable = await IsEmailAvailable(email);

            return Ok(new { isAvailable = emailAvailable });
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivateAccount([FromQuery] string token)
        {
            // Find the user by the token
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Token == token);

            if (user == null || user.TokenExpDate < DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired activation token.");
            }

            // Activate user account
            user.isActive = true;
            user.Token = null; // clear the token once used
            user.TokenExpDate = DateTime.MinValue;

            await dbContext.SaveChangesAsync();

            return Ok("Account activated successfully.");
        }

        public void SendActivationEmail(UserModel user)
        {
            var fromPassword = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");
            var activationLink = $"https://yourdomain.com/activate?token={user.Token}";
            var fromAddress = new MailAddress("portaldeoportunidades2024@gmail.com", "Portal De Oportunidades");
            var toAddress = new MailAddress(user.Email);
            const string subject = "Activate Your Account";
            string body = $"Hello {user.FirstName} {user.LastName},\n\nPlease click the link below to activate your account:\n{activationLink}\n\nThank you!";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }


    }
}
