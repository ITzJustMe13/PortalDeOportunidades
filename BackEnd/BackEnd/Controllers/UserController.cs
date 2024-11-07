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

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ApplicationDbContext dbContext;
        public UserController(ApplicationDbContext dbContext) => this.dbContext = dbContext;

        private EmailService emailService;

        // GET: api/User/{id}
        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserByID(int id)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
                return NotFound("User was not found!");

            var u = UserMapper.MapToDto(user);

            return Ok(u);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> CreateNewUser(User user)
        {
            if (dbContext == null)
                return NotFound("DB context missing");


            // Verifica se o dto é valido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await IsEmailAvailable(user.email))
            {
                return BadRequest("Email is already in use");
            }

            // validar se o gender existe
            if (!Enum.IsDefined(typeof(Gender), user.gender))
            {
                return BadRequest("Gender has an invalid value");
            }

            if (user.password == null)
            {
                return BadRequest("Password is required");
            }

            // validar se o utilizador tem pelo menos 18 anos
            if ((DateTime.Now.Year - user.birthDate.Year) < 18)
            {
                return BadRequest("You must be at least 18 years old");
            }

            if (user.IBAN != null) 
            {
                if (!IBANService.ValidateIBAN(user.IBAN))
                {
                    return BadRequest("IBAN is not valid");
                }
            }

            // encripta a password do utilizador
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(user.password, 13);

            try
            {
                var u = UserMapper.MapToModel(user);

                if (u == null)
                    return Problem("Error converting to Model");

                u.HashedPassword = passwordHash;
                u.isActive = false;

                var activationToken = AuthService.GenerateToken(u);
                u.Token = activationToken;
                u.TokenExpDate = DateTime.UtcNow.AddHours(24);
                await dbContext.SaveChangesAsync();
                EmailService.SendActivationEmail(u);

                await dbContext.Users.AddAsync(u);
                await dbContext.SaveChangesAsync();
                var createdUserDTO = UserMapper.MapToDto(u);


                if (createdUserDTO == null)
                    return Problem("Error converting to DTO");

                return CreatedAtAction(nameof(GetUserByID), new { createdUserDTO.userId }, createdUserDTO);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //DELETE: api/User/2
        
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(int id)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
                return NotFound("The user was not Found!");

            // encontra e elimina as reservas, oportunidades e impulsos do utilizador
            var reservas = await dbContext.Reservations.Where(r => r.userID == id).ToListAsync();
            var oportunidades = await dbContext.Opportunities.Where(o => o.UserID == id).ToListAsync();
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
        [Authorize]
        public async Task<ActionResult<User>> EditUser(int id, User updatedUser)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            // Find the existing user by ID
            var existingUser = await dbContext.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound("User Not Found!");

            if (!existingUser.Email.Equals(updatedUser.email))
            {
                if (!await IsEmailAvailable(updatedUser.email))
                {
                    return BadRequest("The email is already used");
                }
            }

            // validar se o gender posto existe
            if (!Enum.IsDefined(typeof(Gender), updatedUser.gender))
            {
                return BadRequest("The gender is invalid");
            }

            // validar se o utilizador tem pelo menos 18 anos
            if ((DateTime.Now.Year - updatedUser.birthDate.Year) < 18)
            {
                return BadRequest("You must be atleast 18");
            }
            // valida se o utilizador tem iban e se este é valido
            if (updatedUser.IBAN != null)
            {
                if (!IBANService.ValidateIBAN(updatedUser.IBAN))
                {
                    return BadRequest("IBAN is not valid");
                }
            }

            // Atualizar as informa��es do utilizador existente com as novas informa��es
            existingUser.FirstName = updatedUser.firstName;
            existingUser.LastName = updatedUser.lastName;
            existingUser.Email = updatedUser.email;
            existingUser.CellPhoneNum = updatedUser.cellPhoneNumber;
            existingUser.BirthDate = updatedUser.birthDate;
            existingUser.Gender = updatedUser.gender;
            existingUser.Image = updatedUser.image;

            if (!String.IsNullOrEmpty(updatedUser.password))
            {
                string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(updatedUser.password, 13);
                existingUser.HashedPassword = passwordHash;
            }

            try
            {
                dbContext.Users.Update(existingUser);
                await dbContext.SaveChangesAsync();

                var userDTO = UserMapper.MapToDto(existingUser);

                if (userDTO == null)
                {
                    return Problem("The conversion to DTO failed");
                }

                return Ok(userDTO);
            }
            catch (ValidationException ve)
            {
                return BadRequest(ve.Message);
            }
        }

        // POST: api/user/add-favorite
        [Authorize]
        [HttpPost("favorite")]
        public async Task<ActionResult<Favorite>> AddFavorite(Favorite favorite)
        {
            if (dbContext == null)
                return NotFound("DB context missing");


            if (favorite.userId <= 0 || favorite.opportunityId == 0)
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
        [Authorize]
        [HttpGet("favorite/{userId}/{opportunityId}")]
        public async Task<ActionResult<Favorite>> GetFavoriteById(int userId, int opportunityId)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            if (userId <= 0 || opportunityId <= 0)
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
        [Authorize]
        [HttpGet("favorites/{userId}")]
        public async Task<ActionResult<Favorite[]>> GetFavorites(int userId)
        {
            if (dbContext == null)
                return NotFound("DB context missing");


            if (userId <= 0)
            {
                return BadRequest("Invalid userId");
            }

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

        // POST: api/user/impulse
        [Authorize]
        [HttpPost("impulse")]
        public ActionResult<Impulse> ImpulseOportunity(Impulse impulse)
        {
            if (dbContext == null)
                return NotFound("DB context missing");


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var i= null as ImpulseModel;

            try
            {
                 i = ImpulseMapper.MapToModel(impulse);

                if (i == null)
                {
                    return Problem("The conversion to model failed.");
                }
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

                if (impulse.value <= 0)
                {
                    return BadRequest("The value has to be more than 0.");
                }

                if (DateTime.Now.CompareTo(impulse.expireDate) >= 0) {
                    return BadRequest("The impulse expire date has to be in the future.");
                }

                dbContext.Impulses.Add(i);
                dbContext.SaveChanges();

                var impulseDTO = ImpulseMapper.MapToDto(i);

                if (impulseDTO == null)
                {
                    return Problem("The conversion to DTO failed.");
                }

                return CreatedAtAction(nameof(ImpulseOportunity), new { impulseDTO.userId, impulseDTO.opportunityId }, impulseDTO);
        
        }

        // GET: api/User/created-opportunities/{id}
        [Authorize]
        [HttpGet("created-opportunities/{userId}")]
        public async Task<ActionResult<Favorite[]>> GetCreatedOpportunities(int userId)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            if (userId <= 0) 
            {
                return BadRequest("Invalid userId");
            }

            var opportunities = await dbContext.Opportunities.Where(o => o.UserID == userId).ToListAsync();

            var opportunitiesDTOs = opportunities.Select(o => new Favorite
            {
                userId = o.UserID,
                opportunityId = o.OpportunityId,
            }).ToArray();

            if (opportunitiesDTOs.Length == 0)
            {
                return NotFound("You have no created Opportunities!");
            }

            return Ok(opportunitiesDTOs);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (dbContext == null)
                return NotFound("DB context missing");


            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username or password is null");

            var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("The autentication failed! Please check your credentials.");

            if (!user.isActive)
            {
                return BadRequest("Account is dectivated. Please check your email for the activation link.");
            }

            if (!BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.HashedPassword))
            {
                return Unauthorized("The autentication failed! Please check your credentials.");
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
            if (dbContext == null)
                return NotFound("DB context missing");

            var emailAvailable = await IsEmailAvailable(email);

            return Ok( emailAvailable );
        }

        [HttpGet("activate")]
        public async Task<IActionResult> ActivateAccount([FromQuery]string token)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            // Encontrar o utilizador pelo Token
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Token == token);

            if (user == null || user.TokenExpDate < DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired activation token.");
            }

            // Ativar a conta do utilizador
            user.isActive = true;
            user.Token = null;
            user.TokenExpDate = null;

            dbContext.Users.Update(user);

            await dbContext.SaveChangesAsync();

            return Ok("Account activated successfully.");
        }



    }
}
