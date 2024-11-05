using BackEnd.Controllers.Data;
using BackEnd.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.Models.FrontEndModels;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Models.BackEndModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Stripe;
using DotNetEnv;


namespace BackEnd.Test
{
    [TestFixture]
    public class PaymentControllerTest
    {
        private PaymentController _controller;
        private ApplicationDbContext _context;
        private string stripeKey;

        [SetUp]
        public void Setup()
        {
            // Use in-memory database for ApplicationDbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new PaymentController(_context);
            
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateReservationCheckoutSession_ReturnsOkWithCheckoutSessionId_ForValidReservation()
        {
            // Arrange
            DotNetEnv.Env.Load(@"../BackEnd.Test/.env");
            stripeKey = Env.GetString("STRIPE_SECRET_KEY");
            StripeConfiguration.ApiKey = stripeKey;
            byte[] userImg = new byte[]
            {
        137, 80, 78, 71, 13, 10, 26, 10,
        0, 0, 0, 13, 73, 72, 68, 82,
        0, 0, 0, 1, 0, 0, 0, 1,
        8, 6, 0, 0, 0, 197, 158, 108,
        0, 0, 0, 1, 115, 90, 129,
        1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0
            };

            var user = new UserModel
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = DateTime.Now.AddYears(-30),
                CellPhoneNum = 919919919,
                Email = "example@email.com",
                Gender = Enums.Gender.MASCULINO,
                Image = userImg
            };

            _context.Users.Add(user);
            var opportunity = new OpportunityModel
            {
                OpportunityId = 1,
                Price = 100,
                Address = "um sitio",
                Category = Enums.Category.AGRICULTURA,
                userID = 1,
                Name = "name",
                Description = "a description",
                date = DateTime.Now.AddDays(30),
                Vacancies = 2,
                IsActive = true,
                Location = Enums.Location.LISBOA,
                Score = 0,
                IsImpulsed = false
            };

            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user.UserId,
                reservationDate = DateTime.Now.Date,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };

            // Act

            var response = await _controller.CreateReservationCheckoutSession(reservation);

            // Assert
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo(stripeKey), "Error message should match the expected bad request message");
            var OkRequestResult = response as OkObjectResult;
            Assert.That(OkRequestResult, Is.Not.Null);
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateReservationCheckoutSession_ReturnsBadRequest_ForEmptyReservation()
        {
            // Arrange
            Reservation invalidReservation = null;

            // Act
            var response = await _controller.CreateReservationCheckoutSession(invalidReservation);

            // Assert
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid reservation data."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateReservationCheckoutSession_ReturnsBadRequest_ForReservationFixedPriceLessOrEqualTo0()
        {
            // Arrange
            byte[] userImg = new byte[]
            {
                137, 80, 78, 71, 13, 10, 26, 10,
                0, 0, 0, 13, 73, 72, 68, 82,
                0, 0, 0, 1, 0, 0, 0, 1,
                8, 6, 0, 0, 0, 197, 158, 108,
                0, 0, 0, 1, 115, 90, 129,
                1, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0
            };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = userImg };
            _context.Users.Add(user);
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();
            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user.UserId,
                reservationDate = DateTime.Now.Date,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = -1
            };

            // Act
            var response = await _controller.CreateReservationCheckoutSession(reservation);

            // Assert
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid reservation data."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateReservationCheckoutSession_ReturnsNotFound_ForReservationUserNotFound()
        {
            // Arrange
            var nonExistentUserId = 1;
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = nonExistentUserId,
                reservationDate = DateTime.Now.Date,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };

            // Act
            var response = await _controller.CreateReservationCheckoutSession(reservation);

            // Assert
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult?.Value, Is.EqualTo("User not found."));

        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateReservationCheckoutSession_ReturnsNotFound_ForReservationOpportunityNotFound()
        {
            // Arrange
            byte[] userImg = new byte[]
            {
                137, 80, 78, 71, 13, 10, 26, 10,
                0, 0, 0, 13, 73, 72, 68, 82,
                0, 0, 0, 1, 0, 0, 0, 1,
                8, 6, 0, 0, 0, 197, 158, 108,
                0, 0, 0, 1, 115, 90, 129,
                1, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0
            };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image=userImg };
            var nonExistenOpportunityId = 1; 
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = nonExistenOpportunityId,
                userId = user.UserId,
                reservationDate = DateTime.Now.Date,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };

            // Act
            var response = await _controller.CreateReservationCheckoutSession(reservation);

            // Assert
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult?.Value, Is.EqualTo("Opportunity not found."));

        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateImpulseCheckoutSession_ReturnsBadRequest_ForNullImpulseDto()
        {
            // Arrange
            Impulse impulse = null;

            // Act
            var response = await _controller.CreateImpulseCheckoutSession(impulse);

            // Assert
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid impulse data."));

        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateImpulseCheckoutSession_ReturnsBadRequest_ForValueLessOrEqualTo0()
        {
            // Arrange
            byte[] userImg = new byte[]
            {
                137, 80, 78, 71, 13, 10, 26, 10,
                0, 0, 0, 13, 73, 72, 68, 82,
                0, 0, 0, 1, 0, 0, 0, 1,
                8, 6, 0, 0, 0, 197, 158, 108,
                0, 0, 0, 1, 115, 90, 129,
                1, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0
            };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = userImg };
            _context.Users.Add(user);
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();
            var date = DateTime.Today;
            var impulse = new Impulse
            {
                userId = user.UserId,
                opportunityId = opportunity.OpportunityId,
                value = 0,
                expireDate = date.AddDays(30)
            };

            // Act
            var response = await _controller.CreateImpulseCheckoutSession(impulse);

            // Assert
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid impulse data."));
  
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateImpulseCheckoutSession_ReturnsBadRequest_ForDateBeforeTomorrow()
        {
            // Arrange
            byte[] userImg = new byte[]
            {
                137, 80, 78, 71, 13, 10, 26, 10,
                0, 0, 0, 13, 73, 72, 68, 82,
                0, 0, 0, 1, 0, 0, 0, 1,
                8, 6, 0, 0, 0, 197, 158, 108,
                0, 0, 0, 1, 115, 90, 129,
                1, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0
            };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = userImg };
            _context.Users.Add(user);
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();
            var date = DateTime.Today;
            var impulse = new Impulse
            {
                userId = user.UserId,
                opportunityId = opportunity.OpportunityId,
                value = 10,
                expireDate = date.AddDays(-30)
            };

            // Act
            var response = await _controller.CreateImpulseCheckoutSession(impulse);

            // Assert
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid impulse data."));

        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateImpulseCheckoutSession_ReturnsBadRequest_ForDateOnTheSameDay()
        {
            // Arrange
            byte[] userImg = new byte[]
            {
                137, 80, 78, 71, 13, 10, 26, 10,
                0, 0, 0, 13, 73, 72, 68, 82,
                0, 0, 0, 1, 0, 0, 0, 1,
                8, 6, 0, 0, 0, 197, 158, 108,
                0, 0, 0, 1, 115, 90, 129,
                1, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0
            };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = userImg };
            _context.Users.Add(user);
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();
            var date = DateTime.Today;
            var impulse = new Impulse
            {
                userId = user.UserId,
                opportunityId = opportunity.OpportunityId,
                value = 10,
                expireDate = date
            };

            // Act
            var response = await _controller.CreateImpulseCheckoutSession(impulse);

            // Assert
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid impulse data."));

        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateImpulseCheckoutSession_ReturnsNotFound_ForNonExistentUser()
        {
            // Arrange
            var nonExistentUserId = 1;
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();
            var date = DateTime.Today;
            var impulse = new Impulse
            {
                userId = nonExistentUserId,
                opportunityId = opportunity.OpportunityId,
                value = 10,
                expireDate = date
            };

            // Act
            var response = await _controller.CreateImpulseCheckoutSession(impulse);

            // Assert
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid impulse data."));

        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateImpulseCheckoutSession_ReturnsNotFound_ForNonExistentOpportunity()
        {
            // Arrange
            byte[] userImg = new byte[]
            {
                137, 80, 78, 71, 13, 10, 26, 10,
                0, 0, 0, 13, 73, 72, 68, 82,
                0, 0, 0, 1, 0, 0, 0, 1,
                8, 6, 0, 0, 0, 197, 158, 108,
                0, 0, 0, 1, 115, 90, 129,
                1, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0
            };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = userImg };
            var nonExistenOpportunityId = 1;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var date = DateTime.Today;
            var impulse = new Impulse
            {
                userId = user.UserId,
                opportunityId = nonExistenOpportunityId,
                value = 10,
                expireDate = date
            };

            // Act
            var response = await _controller.CreateImpulseCheckoutSession(impulse);

            // Assert
            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid impulse data."));

        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateImpulseCheckoutSession_ReturnsOkWithCheckoutSessionId_ForValidImpulse()
        {
            // Arrange
            
            StripeConfiguration.ApiKey = stripeKey;
            byte[] userImg = new byte[]
            {
        137, 80, 78, 71, 13, 10, 26, 10,
        0, 0, 0, 13, 73, 72, 68, 82,
        0, 0, 0, 1, 0, 0, 0, 1,
        8, 6, 0, 0, 0, 197, 158, 108,
        0, 0, 0, 1, 115, 90, 129,
        1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0
            };

            var user = new UserModel
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = DateTime.Now.AddYears(-30),
                CellPhoneNum = 919919919,
                Email = "example@email.com",
                Gender = Enums.Gender.MASCULINO,
                Image = userImg
            };

            _context.Users.Add(user);
            var opportunity = new OpportunityModel
            {
                OpportunityId = 1,
                Price = 100,
                Address = "um sitio",
                Category = Enums.Category.AGRICULTURA,
                userID = 1,
                Name = "name",
                Description = "a description",
                date = DateTime.Now.AddDays(30),
                Vacancies = 2,
                IsActive = true,
                Location = Enums.Location.LISBOA,
                Score = 0,
                IsImpulsed = false
            };

            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();

            var date = DateTime.Today;
            var impulse = new Impulse
            {
                userId = user.UserId,
                opportunityId = opportunity.OpportunityId,
                value = 10,
                expireDate = date.AddDays(30)
            };

            // Act

            var response = await _controller.CreateImpulseCheckoutSession(impulse);

            // Assert

            var OkRequestResult = response as OkObjectResult;
            Assert.That(OkRequestResult, Is.Not.Null);
        }
    }
}
