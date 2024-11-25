using Azure;
using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.ServiceResponses;
using BackEnd.Interfaces;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using BackEnd.Responses;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Stripe;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Services
{
    /// <summary>
    /// The class is responsible for the logic of Users of the program
    /// and implements the IUserService Interface
    /// Has a constructor that receives a DBContext an IEmailService and an IIbanService
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IEmailService emailService;
        private readonly IAuthService authService;
        private readonly IIBanService ibanService;

        public UserService(
            ApplicationDbContext dbContext,
            IEmailService emailService,
            IAuthService authService,
            IIBanService ibanService)
        {
            this.dbContext = dbContext;
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.ibanService = ibanService ?? throw new ArgumentNullException(nameof(ibanService)) ;
        }

        /// <summary>
        /// Function responsible to get from the db a Users info by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and the User Dto</returns>
        public async Task<ServiceResponse<User>> GetUserByIDAsync(int id)
        {
            if (dbContext == null)
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "DB context is Missing",
                    Type = "NotFound"
                };

            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "User was not found!",
                    Type = "NotFound"
                };

            var u = UserMapper.MapToDto(user);

            return new ServiceResponse<User>
            {
                Success = true,
                Data = u
            };
        }

        /// <summary>
        /// Function that creates a new User in the DB
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and the created User dto</returns>
        public async Task<ServiceResponse<User>> CreateNewUserAsync(User user)
        {
            var response = new ServiceResponse<User>();

            if (dbContext == null)
            {
                response.Success = false;
                response.Message = "DB context missing";
                response.Type = "NotFound";
                return response;
            }

            if (user == null)
            {
                response.Success = false;
                response.Message = "User cannot be null";
                response.Type = "BadRequest";
                return response;
            }
            
            if (!await IsEmailAvailable(user.email))
            {
                response.Success = false;
                response.Message = "Email is already in use";
                response.Type = "BadRequest";
                return response;
            }

            if (!Enum.IsDefined(typeof(Gender), user.gender))
            {
                response.Success = false;
                response.Message = "Gender has an invalid value";
                response.Type = "BadRequest";
                return response;
            }

            if (string.IsNullOrEmpty(user.password))
            {
                response.Success = false;
                response.Message = "Password is required";
                response.Type = "BadRequest";
                return response;
            }

            if ((DateTime.Now.Year - user.birthDate.Year) < 18)
            {
                response.Success = false;
                response.Message = "You must be at least 18 years old";
                response.Type = "BadRequest";
                return response;
            }

            if (!string.IsNullOrEmpty(user.IBAN) && !ibanService.ValidateIBAN(user.IBAN))
            {
                response.Success = false;
                response.Message = "IBAN is not valid";
                response.Type = "BadRequest";
                return response;
            }

            // Encripta a password do utilizador
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(user.password, 13);

            try
            {
                var u = UserMapper.MapToModel(user);
                
                if (u == null)
                {
                    response.Success = false;
                    response.Message = "Error converting to Model";
                    response.Type = "BadRequest";
                    return response;
                }

                u.HashedPassword = passwordHash;
                u.isActive = false;

                var activationToken = authService.GenerateToken(u);
                u.Token = activationToken;
                u.TokenExpDate = DateTime.UtcNow.AddHours(24);

                await dbContext.Users.AddAsync(u);
                await dbContext.SaveChangesAsync();

                emailService.SendActivationEmail(u);

                var createdUserDTO = UserMapper.MapToDto(u);
                if (createdUserDTO == null)
                {
                    response.Success = false;
                    response.Message = "Error converting to DTO";
                    response.Type = "BadRequest";
                    return response;
                }

                response.Data = createdUserDTO;
                response.Success = true;
                response.Message = "User successfully created";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Type = "BadRequest";
            }

            return response;
        }

        /// <summary>
        /// Function that deletes the user from the DB by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and a sucess mensage</returns>
        public async Task<ServiceResponse<string>> DeleteUserAsync(int id)
        {
            var response = new ServiceResponse<string>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                var user = await dbContext.Users.FindAsync(id);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "The user was not found.";
                    response.Type = "NotFound";
                    return response;
                }

                // Encontrar e eliminar as reservas, oportunidades e impulsos do utilizador
                var reservas = await dbContext.Reservations.Where(r => r.userID == id).ToListAsync();
                var oportunidades = await dbContext.Opportunities.Where(o => o.UserID == id).ToListAsync();
                var impulses = await dbContext.Impulses.Where(i => i.UserId == id).ToListAsync();

                dbContext.Users.Remove(user);
                dbContext.Reservations.RemoveRange(reservas);
                dbContext.Opportunities.RemoveRange(oportunidades);
                dbContext.Impulses.RemoveRange(impulses);

                await dbContext.SaveChangesAsync();

                response.Success = true;
                response.Message = "User and related data deleted successfully.";
                response.Data = $"User with ID {id} deleted.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while deleting the user.";
                response.Type = "BadRequest";
            }

            return response;
        }

        /// <summary>
        /// Function that updates the user based on the updatedUser dto that it receives
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedUser"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and a sucess mensage</returns>
        public async Task<ServiceResponse<User>> EditUserAsync(int id, User updatedUser)
        {
            var response = new ServiceResponse<User>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                var existingUser = await dbContext.Users.FindAsync(id);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    response.Type = "NotFound";

                    return response;
                }

                if (!existingUser.Email.Equals(updatedUser.email))
                {
                    if (!await IsEmailAvailable(updatedUser.email))
                    {
                        response.Success = false;
                        response.Message = "The email is already in use.";
                        response.Type = "BadRequest";
                        return response;
                    }
                }

                // Validar se o gender fornecido existe
                if (!Enum.IsDefined(typeof(Gender), updatedUser.gender))
                {
                    response.Success = false;
                    response.Message = "Invalid gender.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Validar idade mínima
                if ((DateTime.Now.Year - updatedUser.birthDate.Year) < 18)
                {
                    response.Success = false;
                    response.Message = "The user must be at least 18 years old.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Validar IBAN se fornecido
                if (updatedUser.IBAN != null && !ibanService.ValidateIBAN(updatedUser.IBAN))
                {
                    response.Success = false;
                    response.Message = "Invalid IBAN.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Atualizar os dados do usuário
                existingUser.FirstName = updatedUser.firstName;
                existingUser.LastName = updatedUser.lastName;
                existingUser.Email = updatedUser.email;
                existingUser.CellPhoneNum = updatedUser.cellPhoneNumber;
                existingUser.BirthDate = updatedUser.birthDate;
                existingUser.Gender = updatedUser.gender;
                existingUser.Image = updatedUser.image;

                if (!string.IsNullOrEmpty(updatedUser.password))
                {
                    string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(updatedUser.password, 13);
                    existingUser.HashedPassword = passwordHash;
                }

                dbContext.Users.Update(existingUser);
                await dbContext.SaveChangesAsync();

                // Converter para DTO
                var userDTO = UserMapper.MapToDto(existingUser);
                if (userDTO == null)
                {
                    response.Success = false;
                    response.Message = "Conversion to UserDTO failed.";
                    response.Type = "BadRequest";
                    return response;
                }

                response.Data = userDTO;
                response.Success = true;
                response.Message = "User updated successfully.";
            }
            catch (ValidationException ve)
            {
                response.Success = false;
                response.Message = ve.Message;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while updating the user.";
                response.Type = "BadRequest";
            }

            return response;
        }

        /// <summary>
        /// Function that makes the User login
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true with a sucess mensage and a
        /// LoginResponse with a jwt token and the user dto it belongs to</returns>
        public async Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var response = new ServiceResponse<LoginResponse>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    response.Success = false;
                    response.Message = "Email is required.";
                    response.Type = "BadRequest";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    response.Success = false;
                    response.Message = "Password is required.";
                    response.Type = "BadRequest";
                    return response;
                }

                var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.HashedPassword))
                {
                    response.Success = false;
                    response.Message = "The authentication failed! Please check your credentials.";
                    response.Type = "Unauthorized";
                    return response;
                }

                if (!user.isActive)
                {
                    response.Success = false;
                    response.Message = "Account is deactivated. Please check your email for the activation link.";
                    response.Type = "BadRequest";
                    return response;
                }

                var authenticatedUserDTO = UserMapper.MapToDto(user);
                if (authenticatedUserDTO == null)
                {
                    response.Success = false;
                    response.Message = "Error while mapping the UserDTO.";
                    response.Type = "BadRequest";
                    return response;
                }

                var token = authService.GenerateToken(user);

                response.Data = new LoginResponse
                {
                    token = token,
                    user = authenticatedUserDTO
                };
                response.Success = true;
                response.Message = "Login successful.";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred during login.";
                response.Type = "BadRequest";
            }

            return response;
        }

        /// <summary>
        /// Function that checks if an email is available to use, that is, is not present in the DB
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Returns false if already exists a similar email on the DB or the email parameter
        /// is null or empty or returns true if the email doesn't exist on the DB</returns>
        private async Task<Boolean> IsEmailAvailable(string email)
        {

            if (string.IsNullOrWhiteSpace(email))
                return false;

            return !await dbContext.Users.AnyAsync(user => user.Email == email);
        }

        /// <summary>
        /// Function that exists with the purpose of informing the user if his email is already registered
        /// on the plataform
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and a mensage informing the user
        /// about his choice of email availability accordingly </returns>
        public async Task<ServiceResponse<bool>> CheckEmailAvailabilityAsync(string email)

        {
            var response = new ServiceResponse<bool>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    response.Success = false;
                    response.Message = "Email cannot be null or empty.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Verifica a disponibilidade do email
                var emailAvailable = await IsEmailAvailable(email);

                response.Data = emailAvailable;
                response.Success = true;
                response.Message = emailAvailable
                ? "Email is available."
                : "Email is already in use.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while checking email availability.";
            }

            return response;
        }

        /// <summary>
        /// Function that activates the User account on the DB and plataform
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and a sucess mensage</returns>
        public async Task<ServiceResponse<string>> ActivateAccountAsync(string token)
        {
            var response = new ServiceResponse<string>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(token))
                {
                    response.Success = false;
                    response.Message = "Token cannot be null or empty.";
                    return response;
                }

                // Encontrar o utilizador pelo token
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Token == token);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Invalid activation token.";
                    return response;
                }

                if (user.TokenExpDate == null || user.TokenExpDate < DateTime.UtcNow)
                {
                    response.Success = false;
                    response.Message = "The activation token has expired.";
                    return response;
                }

                // Ativar a conta do utilizador
                user.isActive = true;
                user.Token = null;
                user.TokenExpDate = null;

                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync();

                response.Success = true;
                response.Message = "Account activated successfully.";
                response.Data = response.Message;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while activating the account.";
            }

            return response;
        }

        /// <summary>
        /// Function that activates an Impulse on a User's Opportunity
        /// </summary>
        /// <param name="impulse"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true, a sucess mensage and the Impulse Dto</returns>
        public async Task<ServiceResponse<Impulse>> ImpulseOpportunityAsync(Impulse impulse)
        {
            var response = new ServiceResponse<Impulse>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                // Validação de modelo
                if (impulse == null)
                {
                    response.Success = false;
                    response.Message = "Impulse data is invalid.";
                    response.Type = "BadRequest";
                    return response;
                }

                var opportunity = await dbContext.Opportunities.FirstOrDefaultAsync(o => o.UserID == impulse.userId && o.OpportunityId == impulse.opportunityId);
                if (opportunity == null)
                {
                    response.Success = false;
                    response.Message = "User is not the user wich create the opportunity";
                    response.Type = "BadRequest";
                    return response;
                }

                if (impulse.value <= 0)
                {
                    response.Success = false;
                    response.Message = "The value must be more than 0.";
                    response.Type = "BadRequest";
                    return response;
                }

                if (DateTime.Now.CompareTo(impulse.expireDate) >= 0)
                {
                    response.Success = false;
                    response.Message = "The impulse expiration date must be in the future.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Conversão para o modelo
                ImpulseModel impulseModel;
                try
                {
                    impulseModel = ImpulseMapper.MapToModel(impulse);
                    if (impulseModel == null)
                    {
                        response.Success = false;
                        response.Message = "The conversion to model failed.";
                        response.Type = "BadRequest";
                        return response;
                    }
                }
                catch (ValidationException ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                    response.Type = "BadRequest";
                    return response;
                }

                // Adiciona o impulso à base de dados
                opportunity.IsImpulsed = true;

                dbContext.Impulses.Add(impulseModel);
                dbContext.Opportunities.Update(opportunity);
                await dbContext.SaveChangesAsync();

                // Conversão para DTO
                var impulseDTO = ImpulseMapper.MapToDto(impulseModel);
                if (impulseDTO == null)
                {
                    response.Success = false;
                    response.Message = "The conversion to DTO failed.";
                    response.Type = "BadRequest";
                    return response;
                }

                response.Data = impulseDTO;
                response.Success = true;
                response.Message = "Impulse successfully created.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while creating the impulse.";
                response.Type = "BadRequest";
            }

            return response;
        }

    }

    
}

