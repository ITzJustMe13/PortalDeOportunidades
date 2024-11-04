using BackEnd.Controllers.Data;
using BackEnd.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.Models.BackEndModels;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Test
{
    public class UserControllerTest
    {
        private UserController _controller;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new UserController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup the in-memory database after each test
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
            Assert.That(response.Result, Is.TypeOf<OkObjectResult>());
            var okResult = response.Result as OkObjectResult;
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
        [Category ("UnitTest")]
        public async Task GetClientById_ReturnsNotFound_ForNonExistentUserId()
        {
            // Arrange
            var userId = 1;

            // Act
            var response = await _controller.GetUserByID(userId);

            // Assert
            Assert.That(response.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = response.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult?.Value, Is.EqualTo("User was not found!"));
        }
    }
}
