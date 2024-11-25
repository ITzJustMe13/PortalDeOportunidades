using BackEnd.Controllers.Data;
using BackEnd.Controllers;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models.BackEndModels;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Models.FrontEndModels;
using Sprache;
using Microsoft.AspNetCore.Identity.Data;
using BackEnd.Enums;
using Azure;
using DotNetEnv;
using BackEnd.Interfaces;
using BackEnd.Services;
using NUnit.Framework;
using System;
using Stripe;
using BackEnd.Models.Mappers;

namespace BackEnd.Test
{
    public class UserControllerTest
    {
        private IUserService _userService;
        private IFavoritesService _favoritesService;
        private UserController _controller;
        private ApplicationDbContext _context;
        private IEmailService _emailService = new EmailService();
        private IIBanService _ibanService = new IBANService();
        private IAuthService _authService = new AuthService(); 

        [SetUp]
        public void Setup()
        {

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase("TestDatabase")
        .Options;

         _context = new ApplicationDbContext(options);

            _userService = new UserService(_context, _emailService, _authService, _ibanService);

            _favoritesService = new FavoritesService(_context);

            _controller = new UserController(_userService, _favoritesService);


            // Carregar o arquivo .env, se necessário
            string envTestPath = Path.GetFullPath("../../../../BackEnd/.env");
            Console.WriteLine("Resolved .env Path: " + envTestPath);

            if (System.IO.File.Exists(envTestPath))
            {
                try
                {
                    Env.Load(envTestPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading .env: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine(".env file not found at the specified path.");
            }
        }

    [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetClientById_ReturnsOkWithUserDto_ForValidId()
        {
            // Arrange
            var userId = 1;
            var staticDate = new DateTime(2023, 1, 1);
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var userModel = new UserModel
            {
                UserId = userId,
                BirthDate = staticDate,
                RegistrationDate = staticDate,
                CellPhoneNum = 912345678,
                Email = "example@example.com",
                FirstName = "Example",
                LastName = "Test",
                Gender = Enums.Gender.MASCULINO,
                Image = byteArray
            };

            _context.Users.Add(userModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetUserByID(userId);

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>());
            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var returnedUser = okResult.Value as BackEnd.Models.FrontEndModels.User;
            Assert.That(returnedUser, Is.Not.Null);
            Assert.That(returnedUser.userId, Is.EqualTo(userId));
            Assert.That(returnedUser.email, Is.EqualTo("example@example.com"));
            Assert.That(returnedUser.birthDate, Is.EqualTo(staticDate));
            Assert.That(returnedUser.registrationDate, Is.EqualTo(staticDate));
            Assert.That(returnedUser.cellPhoneNumber, Is.EqualTo(912345678));
            Assert.That(returnedUser.firstName, Is.EqualTo("Example"));
            Assert.That(returnedUser.lastName, Is.EqualTo("Test"));
            Assert.That(returnedUser.image, Is.EqualTo(byteArray));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetClientById_ReturnsNotFoundWithUserDto_ForInValidId()
        {
            // Arrange
            var userId = -1;
            var staticDate = new DateTime(2023, 1, 1);
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var userModel = new UserModel
            {
                UserId = 1,
                BirthDate = staticDate,
                RegistrationDate = staticDate,
                CellPhoneNum = 912345678,
                Email = "example@example.com",
                FirstName = "Example",
                LastName = "Test",
                Gender = Enums.Gender.MASCULINO,
                Image = byteArray
            };

            _context.Users.Add(userModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetUserByID(userId);

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("User was not found!"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetUserByID_ReturnsNotFound_WhenDbContextIsMissing()
        {
            //Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService);

            // Act
            var response = await controller.GetUserByID(1);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is Missing"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewUser_ReturnsCreated_WhenUserIsValid()
        {
            // Arrange
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new User
            {
                email = "user@gmail.com",
                password = "ValidPassword123",
                firstName = "Antonio",
                lastName = "Silva",
                cellPhoneNumber = 911232439,
                birthDate = DateTime.Now.AddYears(-20),
                gender = Gender.MASCULINO,
                image = byteArray
                
            };

            // Act
            var response = await _controller.CreateNewUser(user);

            // Assert
            Assert.That(response, Is.TypeOf<CreatedAtActionResult>());
            var createdResult = response as CreatedAtActionResult;
            var createdUser = createdResult?.Value as User;
            Assert.That(createdUser, Is.Not.Null);
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewUser_ReturnsNotFound_WhenDbContextIsNull()
        {
            // Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService);

            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new User
            {
                email = "newuser@gmail.com",
                password = "ValidPassword123",
                firstName = "Antonio",
                lastName = "Silva",
                cellPhoneNumber = 911232439,
                birthDate = DateTime.Now.AddYears(-20), 
                gender = Gender.MASCULINO,
                image = byteArray

            };

            // Act
            var response = await controller.CreateNewUser(user);

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewUser_ReturnsBadRequest_WhenEmailIsAlreadyInUse()
        {
            // Arrange
            byte[] byteArray1 = new byte[] { 72, 101, 108, 108, 111 };
            var existingUser = new UserModel { 
                Email = "existing@example.com",
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("password"), 
                isActive = true, 
                FirstName = "John",
                LastName = "Doe",   
                CellPhoneNum = 912345678,
                RegistrationDate = DateTime.Now, 
                BirthDate = new DateTime(1990, 1, 1), 
                Gender = Gender.MASCULINO, 
                Image = byteArray1 
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            byte[] byteArray2 = new byte[] { 72, 101, 108, 108, 111 };
            var user = new User
            {
                email = "existing@example.com",
                password = "ValidPassword123",
                firstName = "Antonio",
                lastName = "Silva",
                cellPhoneNumber = 911232439,
                birthDate = DateTime.Now.AddYears(-20), 
                gender = Gender.MASCULINO,
                image = byteArray2

            };

            // Act
            var response = await _controller.CreateNewUser(user);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Email is already in use"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewUser_ReturnsBadRequest_WhenGenderIsInvalid()
        {
            // Arrange
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new User
            {
                email = "existing@example.com",
                password = "ValidPassword123",
                firstName = "Antonio",
                lastName = "Silva",
                cellPhoneNumber = 911232439,
                birthDate = DateTime.Now.AddYears(-20),
                gender = (Gender)999,
                image = byteArray

            };

            // Act
            var response = await _controller.CreateNewUser(user);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Gender has an invalid value"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewUser_ReturnsBadRequest_WhenPasswordIsMissing()
        {
            // Arrange
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new User
            {
                email = "existing@example.com",
                password = null,
                firstName = "Antonio",
                lastName = "Silva",
                cellPhoneNumber = 911232439,
                birthDate = DateTime.Now.AddYears(-20), 
                gender = Gender.MASCULINO,
                image = byteArray

            };
            // Act
            var response = await _controller.CreateNewUser(user);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Password is required"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewUser_ReturnsBadRequest_WhenUserIsUnder18()
        {
            // Arrange
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new User
            {
                email = "existing@example.com",
                password = "ValidPassword123",
                firstName = "Antonio",
                lastName = "Silva",
                cellPhoneNumber = 911232439,
                birthDate = DateTime.Now.AddYears(-17), 
                gender = Gender.MASCULINO,
                image = byteArray

            };

            // Act
            var response = await _controller.CreateNewUser(user);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("You must be at least 18 years old"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewUser_ReturnsBadRequest_WhenIBANIsInvalid()
        {
            // Arrange
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new User
            {
                email = "leonardosilva.00009@gmail.com",
                password = "ValidPassword123",
                firstName = "Antonio",
                lastName = "Silva",
                cellPhoneNumber = 911232439,
                birthDate = DateTime.Now.AddYears(-20),
                gender = Gender.MASCULINO,
                image = byteArray,
                IBAN = "NL91ABNA04171643000"

            };

            // Act
            var response = await _controller.CreateNewUser(user);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("IBAN is not valid"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteUser_ReturnsNotFound_ForNonExistentUserId()
        {
            // Arrange
            var userid = 1;

            // Act
            var result = await _controller.DeleteUser(userid);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("The user was not found."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteUser_ReturnsNoContent_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var staticDate = new DateTime(2023, 1, 1);
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var userModel = new UserModel
            {
                UserId = userId,
                BirthDate = staticDate,
                RegistrationDate = staticDate,
                CellPhoneNum = 912345678,
                Email = "example@example.com",
                FirstName = "Example",
                LastName = "Test",
                Gender = Enums.Gender.MASCULINO,
                Image = byteArray
            };

            _context.Users.Add(userModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.DeleteUser(userId);

            // Assert
            Assert.That(response, Is.TypeOf<NoContentResult>());
            Assert.That(await _context.Reservations.FindAsync(userModel.UserId), Is.Null);
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteUser_ReturnsNotFound_WhenDbContextIsMissing()
        {
            //Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService);
            // Act
            var response = await controller.DeleteUser(1);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditUser_ReturnsOk_WhenUserIsValid()
        {
            // Arrange
            int userId = 1;
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var existingUser = new UserModel
            {
                UserId = userId,
                FirstName = "Antonio",
                LastName = "Silva",
                Email = "existinguser@example.com",
                CellPhoneNum = 911232439,
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = Gender.MASCULINO,
                isActive = true,
                Image = byteArray
            };
            _context.Users.Add(existingUser);

            var updatedUser = new User
            {
                userId = userId,
                firstName = "Antonio Updated",
                lastName = "Silva Updated",
                email = "updateduser@example.com",
                cellPhoneNumber = 922334455,
                birthDate = DateTime.Now.AddYears(-25),
                gender = Gender.MASCULINO,
                password = "NewPassword123",
                image = byteArray
            };
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.EditUser(userId, updatedUser);

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>());
            var okResult = response as OkObjectResult;
            Assert.That(okResult?.Value, Is.Not.Null);

            var returnedUser = okResult.Value as User;
            Assert.That(returnedUser?.email, Is.EqualTo(updatedUser.email));
            Assert.That(returnedUser?.firstName, Is.EqualTo(updatedUser.firstName));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 1;
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var updatedUser = new User
            {
                userId = userId,
                firstName = "Antonio Updated",
                lastName = "Silva Updated",
                email = "updateduser@example.com",
                cellPhoneNumber = 922334455,
                birthDate = DateTime.Now.AddYears(-25),
                gender = Gender.MASCULINO,
                password = "NewPassword123",
                image = byteArray
            };

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.EditUser(userId, updatedUser);

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("User not found."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditUser_ReturnsBadRequest_WhenEmailAlreadyInUse()
        {
            // Arrange
            int userId = 1;
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var existingUser1 = new UserModel
            {
                UserId = userId,
                FirstName = "Antonio",
                LastName = "Silva",
                Email = "existinguser1@example.com",
                CellPhoneNum = 911232439,
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = Gender.MASCULINO,
                isActive = true,
                Image = byteArray
            };

            var existingUser2 = new UserModel
            {
                UserId = 2,
                FirstName = "Antonio",
                LastName = "Silva",
                Email = "existinguser2@example.com",
                CellPhoneNum = 911232439,
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = Gender.MASCULINO,
                isActive = true,
                Image = byteArray
            };
            _context.Users.Add(existingUser1);
            _context.Users.Add(existingUser2);

            var updatedUser = new User
            {
                userId = userId,
                firstName = "Antonio Updated",
                lastName = "Silva Updated",
                email = "existinguser2@example.com",
                cellPhoneNumber = 922334455,
                birthDate = DateTime.Now.AddYears(-25),
                gender = Gender.MASCULINO,
                password = "NewPassword123",
                image = byteArray
            };
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.EditUser(userId, updatedUser);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("The email is already in use."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditUser_ReturnsBadRequest_WhenUserIsUnderage()
        {
            // Arrange
            int userId = 1;
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var existingUser = new UserModel
            {
                UserId = userId,
                FirstName = "Antonio",
                LastName = "Silva",
                Email = "existinguser1@example.com",
                CellPhoneNum = 911232439,
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = Gender.MASCULINO,
                isActive = true,
                Image = byteArray
            };
            _context.Users.Add(existingUser);

            var updatedUser = new User
            {
                userId = userId,
                firstName = "Antonio Updated",
                lastName = "Silva Updated",
                email = "existinguser2@example.com",
                cellPhoneNumber = 922334455,
                birthDate = DateTime.Now.AddYears(-17),
                gender = Gender.MASCULINO,
                password = "NewPassword123",
                image = byteArray
            };
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.EditUser(userId, updatedUser);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("The user must be at least 18 years old."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditUser_ReturnsBadRequest_WhenGenderIsInvalid()
        {
            // Arrange
            int userId = 1;
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var existingUser = new UserModel
            {
                UserId = userId,
                FirstName = "Antonio",
                LastName = "Silva",
                Email = "existinguser1@example.com",
                CellPhoneNum = 911232439,
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = Gender.MASCULINO,
                isActive = true,
                Image = byteArray
            };
            _context.Users.Add(existingUser);

            var updatedUser = new User
            {
                userId = userId,
                firstName = "Antonio Updated",
                lastName = "Silva Updated",
                email = "existinguser2@example.com",
                cellPhoneNumber = 922334455,
                birthDate = DateTime.Now.AddYears(-17),
                gender = (Gender)999,
                password = "NewPassword123",
                image = byteArray
            };
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.EditUser(userId, updatedUser);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid gender."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditUser_ReturnsNotFound_WhenDbContextIsMissing()
        {
            //Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService);

            int userId = 1;
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var updatedUser = new User
            {
                userId = userId,
                firstName = "Antonio Updated",
                lastName = "Silva Updated",
                email = "existinguser2@example.com",
                cellPhoneNumber = 922334455,
                birthDate = DateTime.Now.AddYears(-17),
                gender = (Gender)999,
                password = "NewPassword123",
                image = byteArray
            };

            // Act
            var response = await controller.EditUser(userId, updatedUser);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditUser_ReturnsBadRequest_WhenIBANIsInvalid()
        {
            // Arrange
            int userId = 1;
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var existingUser = new UserModel
            {
                UserId = userId,
                FirstName = "Antonio",
                LastName = "Silva",
                Email = "existinguser@example.com",
                CellPhoneNum = 911232439,
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = Gender.MASCULINO,
                isActive = true,
                Image = byteArray
            };
            _context.Users.Add(existingUser);

            var updatedUser = new User
            {
                userId = userId,
                firstName = "Antonio Updated",
                lastName = "Silva Updated",
                email = "updateduser@example.com",
                cellPhoneNumber = 922334455,
                birthDate = DateTime.Now.AddYears(-25),
                gender = Gender.MASCULINO,
                password = "NewPassword123",
                image = byteArray,
                IBAN = "NL91ABNA04171643000"
            };
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.EditUser(userId, updatedUser);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid IBAN."));

        }

        [Test]
        [Category("UnitTest")]
        public async Task AddFavorite_ReturnsBadRequest_WhenUserId()
        {
            // Arrange
            var favorite = new Favorite
            {
                userId = 0,
                opportunityId = 1
            };

            // Act
            var response = await _controller.AddFavorite(favorite);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid user or opportunity ID."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task AddFavorite_ReturnsBadRequest_WhenOpportunityIdIsZero()
        {
            // Arrange
            var favorite = new Favorite
            {
                userId = 1,
                opportunityId = 0
            };

            // Act
            var response = await _controller.AddFavorite(favorite);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value.ToString(), Is.EqualTo("Invalid user or opportunity ID."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task AddFavorite_ReturnsConflict_WhenFavoriteAlreadyExists()
        {
            // Arrange
            var favorite = new Favorite
            {
                userId = 1,
                opportunityId = 1
            };

            var existingFavorite = new FavoritesModel
            {
                UserId = 1,
                OpportunityId = 1
            };

            _context.Favorites.Add(existingFavorite);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.AddFavorite(favorite);

            // Assert
            Assert.That(response, Is.TypeOf<ConflictObjectResult>());
            var conflictResult = response as ConflictObjectResult;
            Assert.That(conflictResult?.Value, Is.EqualTo("Favorite already exists for this user and opportunity."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task AddFavorite_ReturnsCreatedAtAction_WhenFavoriteIsAddedSuccessfully()
        {
            // Arrange
            var favorite = new Favorite
            {
                userId = 1,
                opportunityId = 2
            };

            // Act
            var response = await _controller.AddFavorite(favorite);

            // Assert
            Assert.That(response, Is.TypeOf<CreatedAtActionResult>());
            var createdResult = response as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult.RouteValues?["userId"], Is.EqualTo(favorite.userId));
            Assert.That(createdResult.RouteValues?["opportunityId"], Is.EqualTo(favorite.opportunityId));

            // Verifique se o favorito foi realmente adicionado ao banco de dados
            var addedFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == favorite.userId && f.OpportunityId == favorite.opportunityId);
            Assert.That(addedFavorite, Is.Not.Null);
        }

        [Test]
        [Category("UnitTest")]
        public async Task AddFavorite_ReturnsNotFound_WhenDbContextIsMissing()
        {
            //Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService); ;

            var favorite = new Favorite
            {
                userId = 1,
                opportunityId = 2
            };

            // Act
            var response = await controller.AddFavorite(favorite);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetFavoriteById_ReturnsBadRequest_WhenUserId()
        {
            // Arrange
            var userId = 0;
            var opportunityId = 1;

            // Act
            var response = await _controller.GetFavoriteById(userId, opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid user or opportunity ID."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetFavoriteById_ReturnsBadRequest_WhenOpportunityIdIsZero()
        {
            // Arrange
            var userId = 0;
            var opportunityId = 1;

            // Act
            var response = await _controller.GetFavoriteById(userId, opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid user or opportunity ID."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetFavoriteById_ReturnsNotFound_WhenFavoriteDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var opportunityId = 1;
            var existingFavorite = await _context.Favorites.FindAsync(userId, opportunityId);
            if (existingFavorite != null)
            {
                _context.Favorites.Remove(existingFavorite);
                await _context.SaveChangesAsync();
            }

            // Act
            var response = await _controller.GetFavoriteById(userId, opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetFavoriteById_ReturnsOkWithFavorite_WhenFavoriteExists()
        {
            // Arrange
            var userId = 1;
            var opportunityId = 1;

            var favorite = new FavoritesModel
            {
                UserId = userId,
                OpportunityId = opportunityId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetFavoriteById(userId, opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>());
            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var returnedFavorite = okResult.Value as Favorite;
            Assert.That(returnedFavorite, Is.Not.Null);
            Assert.That(returnedFavorite.userId, Is.EqualTo(userId));
            Assert.That(returnedFavorite.opportunityId, Is.EqualTo(opportunityId));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetFavoriteById_ReturnsNotFound_WhenDbContextIsMissing()
        {
            //Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService);

            // Act
            var response = await controller.GetFavoriteById(1, 1);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetFavorites_ReturnsBadRequest_WhenUserIdIsInvalid()
        {
            // Arrange
            var userId = 0; 

            // Act
            var response = await _controller.GetFavorites(userId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid userId."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetFavorites_ReturnsNotFound_WhenUserHasNoFavorites()
        {
            // Arrange
            var userId = 1;
            var existingFavorites = await _context.Favorites.Where(f => f.UserId == userId).ToListAsync();
            if (existingFavorites.Any())
            {
                _context.Favorites.RemoveRange(existingFavorites);
                await _context.SaveChangesAsync();
            }

            // Act
            var response = await _controller.GetFavorites(userId);

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("No favorites found!"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetFavorites_ReturnsOkWithFavorites_WhenUserHasFavorites()
        {
            // Arrange
            var userId = 1;
            var favorite1 = new FavoritesModel { UserId = userId, OpportunityId = 1 };
            var favorite2 = new FavoritesModel { UserId = userId, OpportunityId = 2 };

            _context.Favorites.AddRange(favorite1, favorite2);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetFavorites(userId);

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>());
            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var returnedFavorites = okResult.Value as Favorite[];
            Assert.That(returnedFavorites, Is.Not.Null);
            Assert.That(returnedFavorites.Length, Is.EqualTo(2));
            Assert.That(returnedFavorites.Any(f => f.opportunityId == 1));
            Assert.That(returnedFavorites.Any(f => f.opportunityId == 2));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetFavorites_ReturnsNotFound_WhenDbContextIsMissing()
        {
            //Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService);

            // Act
            var response = await controller.GetFavorites(1);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public void ImpulseOportunity_ReturnsNotFound_WhenDbContextIsNull()
        {
            // Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService);

            var impulse = new Impulse
            {
                userId = 1,
                opportunityId = 1,
                value = 10,
                expireDate = DateTime.Now.AddDays(1)
            };

            // Act
            var response = controller.ImpulseOportunity(impulse);

            // Assert
            Assert.That(response.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = response.Result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public void ImpulseOportunity_ReturnsBadRequest_WhenValueIsInvalid()
        {
            // Arrange
            var opportunityModel = new OpportunityModel
            {
                OpportunityId = 1,
                Price = 100,
                Address = "um sitio",
                Category = Enums.Category.AGRICULTURA,
                UserID = 1,
                Name = "name",
                Description = "a description",
                Date = DateTime.Now.AddDays(30),
                Vacancies = 2,
                IsActive = true,
                Location = Enums.Location.LISBOA,
                Score = 0,
                IsImpulsed = false
            };
            _context.Opportunities.Add(opportunityModel);
            _context.SaveChangesAsync();
            var impulse = new Impulse { userId = 1, opportunityId = 1, value = 0, expireDate = DateTime.Now.AddDays(1) };

            // Act
            var response = _controller.ImpulseOportunity(impulse);

            // Assert
            Assert.That(response.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response.Result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("The value must be more than 0."));
        }

        [Test]
        [Category("UnitTest")]
        public void ImpulseOportunity_ReturnsBadRequest_WhenExpireDateIsInvalid()
        {
            // Arrange
            var opportunityModel = new OpportunityModel
            {
                OpportunityId = 1,
                Price = 100,
                Address = "um sitio",
                Category = Enums.Category.AGRICULTURA,
                UserID = 1,
                Name = "name",
                Description = "a description",
                Date = DateTime.Now.AddDays(30),
                Vacancies = 2,
                IsActive = true,
                Location = Enums.Location.LISBOA,
                Score = 0,
                IsImpulsed = false
            };
            _context.Opportunities.Add(opportunityModel);
            _context.SaveChangesAsync();
            var impulse = new Impulse { userId = 1, opportunityId = 1, value = 10, expireDate = DateTime.Now.AddMinutes(-1) };

            // Act
            var response = _controller.ImpulseOportunity(impulse);

            // Assert
            Assert.That(response.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response.Result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("The impulse expiration date must be in the future."));
        }

        [Test]
        [Category("UnitTest")]
        public void ImpulseOportunity_ReturnsCreated_WhenImpulseIsValid()
        {
            // Arrange
            var opportunityModel = new OpportunityModel {
                OpportunityId = 1, 
                Price = 100, Address = "um sitio", 
                Category = Enums.Category.AGRICULTURA, 
                UserID = 1, 
                Name = "name", 
                Description = "a description", 
                Date = DateTime.Now.AddDays(30), 
                Vacancies = 2, 
                IsActive = true, 
                Location = Enums.Location.LISBOA,
                Score = 0, 
                IsImpulsed = false 
            };
            _context.Opportunities.Add(opportunityModel);
            _context.SaveChangesAsync();

            var impulse = new Impulse { userId = 1, opportunityId = 1, value = 10, expireDate = DateTime.Now.AddDays(1) };

            // Act
            var response = _controller.ImpulseOportunity(impulse);

            // Assert
            Assert.That(response.Result, Is.TypeOf<CreatedAtActionResult>());
            var createdResult = response.Result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);

            var impulseDTO = createdResult.Value as Impulse;
            Assert.That(impulseDTO, Is.Not.Null);
            Assert.That(impulseDTO.userId, Is.EqualTo(impulse.userId));
            Assert.That(impulseDTO.opportunityId, Is.EqualTo(impulse.opportunityId));
            Assert.That(impulseDTO.value, Is.EqualTo(impulse.value));
            Assert.That(impulseDTO.expireDate, Is.EqualTo(impulse.expireDate));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetCreatedOpportunities_ReturnsBadRequest_WhenUserIdIsInvalid()
        {
            // Arrange

            var userId = 0; 

            // Act
            var response = await _controller.GetCreatedOpportunities(userId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid userId."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetCreatedOpportunities_ReturnsNotFound_WhenNoOpportunitiesExist()
        {
            // Arrange
            int userId = 1;

            // Act
            var response = await _controller.GetCreatedOpportunities(userId);

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("You have no created opportunities!"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetCreatedOpportunities_ReturnsOkWithOpportunities_ForValidUserIdWithOpportunities()
        {
            // Arrange
            int userId = 1;

            var opportunity1 = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "outro sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunity1);
            _context.Opportunities.Add(opportunity2);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetCreatedOpportunities(userId);

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>());
            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var opportunitiesDTOs = okResult.Value as Favorite[];
            Assert.That(opportunitiesDTOs, Is.Not.Null);
            Assert.That(opportunitiesDTOs.Length, Is.EqualTo(2));
            Assert.That(opportunitiesDTOs.Any(o => o.opportunityId == opportunity1.OpportunityId && o.userId == userId));
            Assert.That(opportunitiesDTOs.Any(o => o.opportunityId == opportunity2.OpportunityId && o.userId == userId));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetCreatedOpportunities_ReturnsNotFound_WhenDbContextIsMissing()
        {
            //Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService);

            // Act
            var response = await controller.GetCreatedOpportunities(1);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task Login_ReturnsBadRequest_WhenEmailIsEmpty()
        {
            // Arrange
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new UserModel
            {
                UserId = 1,
                FirstName = "Antonio",
                LastName = "Silva",
                HashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword("password", 13),
                Email = "existinguser1@example.com",
                CellPhoneNum = 911232439,
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = Gender.MASCULINO,
                isActive = true,
                Image = byteArray
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var request = new LoginRequest { Email = "", Password = "password" };

            // Act
            var response = await _controller.Login(request);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Email is required."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task Login_ReturnsBadRequest_WhenPasswordIsEmpty()
        {
            // Arrange
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new UserModel
            {
                UserId = 1,
                FirstName = "Antonio",
                LastName = "Silva",
                HashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword("password", 13),
                Email = "existinguser1@example.com",
                CellPhoneNum = 911232439,
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = Gender.MASCULINO,
                isActive = true,
                Image = byteArray
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var request = new LoginRequest { Email = user.Email, Password = "" };

            // Act
            var response = await _controller.Login(request);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Password is required."));
        }


        [Test]
        [Category("UnitTest")]
        public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var request = new LoginRequest { Email = "nonexistent@example.com", Password = "password" };

            // Act
            var response = await _controller.Login(request);

            // Assert
            Assert.That(response, Is.InstanceOf<UnauthorizedObjectResult>());
            var unauthorizedResult = response as UnauthorizedObjectResult;
            Assert.That(unauthorizedResult?.Value, Is.EqualTo("The authentication failed! Please check your credentials."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task Login_ReturnsBadRequest_WhenAccountIsDeactivated()
        {
            // Arrange
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new UserModel
            {
                Email = "test@example.com",
                HashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword("password", 13),
                isActive = false,
                FirstName = "John", 
                LastName = "Doe",  
                CellPhoneNum = 912345678, 
                RegistrationDate = DateTime.Now, 
                BirthDate = new DateTime(1990, 1, 1), 
                Gender = Gender.MASCULINO, 
                Image = byteArray 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var request = new LoginRequest { Email = user.Email, Password = "password" };

            // Act
            var response = await _controller.Login(request);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Account is deactivated. Please check your email for the activation link."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
        {
            // Arrange
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new UserModel
            {
                Email = "test@example.com",
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("password"), 
                isActive = true, 
                FirstName = "John",
                LastName = "Doe",   
                CellPhoneNum = 912345678, 
                RegistrationDate = DateTime.Now, 
                BirthDate = new DateTime(1990, 1, 1), 
                Gender = Gender.MASCULINO, 
                Image = byteArray 
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var request = new LoginRequest { Email = user.Email, Password = "wrong_password" };

            // Act
            var response = await _controller.Login(request);

            // Assert
            Assert.That(response, Is.InstanceOf<UnauthorizedObjectResult>());
            var unauthorizedResult = response as UnauthorizedObjectResult;
            Assert.That(unauthorizedResult?.Value, Is.EqualTo("The authentication failed! Please check your credentials."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task Login_ReturnsOk_WithTokenAndAuthenticatedUserDto_WhenLoginIsSuccessful()
        {
            // Arrange
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new UserModel
            {
                Email = "test@example.com",
                HashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword("password", 13),
                isActive = true,
                FirstName = "John",
                LastName = "Doe",
                CellPhoneNum = 912345678,
                RegistrationDate = DateTime.Now,
                BirthDate = new DateTime(1990, 1, 1),
                Gender = Gender.MASCULINO,
                Image = byteArray
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var request = new LoginRequest { Email = user.Email, Password = "password" };

            // Act
            var response = await _controller.Login(request);

            // Assert
            Assert.That(response, Is.InstanceOf<OkObjectResult>());
            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
         
        }

        [Test]
        [Category("UnitTest")]
        public async Task Login_ReturnsNotFound_WhenDbContextIsMissing()
        {
            //Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService);

            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new UserModel
            {
                Email = "test@example.com",
                HashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword("password", 13),
                isActive = true,
                FirstName = "John",
                LastName = "Doe",
                CellPhoneNum = 912345678,
                RegistrationDate = DateTime.Now,
                BirthDate = new DateTime(1990, 1, 1),
                Gender = Gender.MASCULINO,
                Image = byteArray
            };

            var request = new LoginRequest { Email = user.Email, Password = "password" };


            // Act
            var response = await controller.Login(request);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetEmailAvailability_ReturnsOk_WithAvailableEmail()
        {
            // Arrange

            string email = "available@example.com";
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new UserModel
            {
                Email = "unavailable@example.com",
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("password"), 
                isActive = true,
                FirstName = "John",
                LastName = "Doe",  
                CellPhoneNum = 912345678,
                RegistrationDate = DateTime.Now, 
                BirthDate = new DateTime(1990, 1, 1), 
                Gender = Gender.MASCULINO, 
                Image = byteArray 
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetEmailAvailability(email);

            // Assert
            Assert.That(response, Is.InstanceOf<OkObjectResult>());
            var okResult = response as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(true));


        }

        [Test]
        [Category("UnitTest")]
        public async Task GetEmailAvailability_ReturnsOk_WithUnavailableEmail()
        {
            // Arrange
            string email = "unavailable@example.com"; 
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new UserModel
            {
                Email = email,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("password"), 
                isActive = true, 
                FirstName = "John", 
                LastName = "Doe",   
                CellPhoneNum = 912345678, 
                RegistrationDate = DateTime.Now,
                BirthDate = new DateTime(1990, 1, 1),
                Gender = Gender.MASCULINO, 
                Image = byteArray 
            };

            // Adiciona o usuário ao contexto
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetEmailAvailability(email);

            // Assert
            Assert.That(response, Is.InstanceOf<OkObjectResult>());
            var okResult = response as OkObjectResult;
            Assert.That(okResult?.Value, Is.Not.Null);

            Assert.That(okResult.Value, Is.EqualTo(false));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetEmailAvailability_ReturnsBadRequest_WhenEmailIsNull()
        {
            // Arrange
            string email = null; // Email nulo

            // Act
            var response = await _controller.GetEmailAvailability(email);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = response as BadRequestObjectResult;
            Assert.That(badRequest?.Value, Is.EqualTo("Email cannot be null or empty."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetEmailAvailability_ReturnsBadRequest_WhenEmailIsEmpty()
        {
            // Arrange
            string email = ""; // Email vazio

            // Act
            var response = await _controller.GetEmailAvailability(email);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = response as BadRequestObjectResult;
            Assert.That(badRequest?.Value, Is.EqualTo("Email cannot be null or empty.")); 
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetEmailAvailability_ReturnsNotFound_WhenDbContextIsMissing()
        {
            //Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService);

            // Act
            var response = await controller.GetEmailAvailability("email@example.pt");

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }


        [Test]
        [Category("UnitTest")]
        public async Task ActivateAccount_ReturnsOk_WhenTokenIsValid()
        {
            // Arrange
            string token = "valid_token";
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new UserModel
            {
                Email = "test@example.com",
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("password"),
                isActive = false, 
                FirstName = "John", 
                LastName = "Doe",   
                CellPhoneNum = 912345678, 
                RegistrationDate = DateTime.Now, 
                BirthDate = new DateTime(1990, 1, 1), 
                Gender = Gender.MASCULINO, 
                Image = byteArray,
                Token = token,
                TokenExpDate = DateTime.UtcNow.AddMinutes(30) 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.ActivateAccount(token);

            // Assert
            Assert.That(response, Is.InstanceOf<OkObjectResult>());
            var okResult = response as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo("Account activated successfully."));

            var updatedUser = await _context.Users.FindAsync(user.UserId);
            Assert.That(updatedUser?.isActive, Is.EqualTo(true));
            Assert.That(updatedUser.Token, Is.EqualTo(null));
            Assert.That(updatedUser.TokenExpDate, Is.EqualTo(null));
        }

        [Test]
        [Category("UnitTest")]
        public async Task ActivateAccount_ReturnsBadRequest_WhenTokenIsInvalid()
        {
            // Arrange
            string token = "invalid_token"; 

            // Act
            var response = await _controller.ActivateAccount(token);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid activation token."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task ActivateAccount_ReturnsBadRequest_WhenTokenIsExpired()
        {
            // Arrange
            string token = "expired_token";
            byte[] byteArray = new byte[] { 72, 101, 108, 108, 111 };
            var user = new UserModel
            {
                Email = "expired@example.com",
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("password"),
                isActive = false,
                FirstName = "John", 
                LastName = "Doe",  
                CellPhoneNum = 912345678, 
                RegistrationDate = DateTime.Now, 
                BirthDate = new DateTime(1990, 1, 1), 
                Gender = Gender.MASCULINO,
                Image = byteArray,
                Token = token,
                TokenExpDate = DateTime.UtcNow.AddMinutes(-10) 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.ActivateAccount(token);

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("The activation token has expired."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task ActivateAccount_ReturnsNotFound_WhenDbContextIsMissing()
        {
            //Arrange
            var userService = new UserService(null, _emailService, _authService, _ibanService);

            var favoritesService = new FavoritesService(null);

            var controller = new UserController(userService, favoritesService);

            // Act
            var response = await controller.ActivateAccount("token");

            // Assert
            Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
            var notFoundResult = response as BadRequestObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

    }
}
