using BackEnd.Controllers;
using BackEnd.Controllers.Data;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Models.BackEndModels;
using Azure;
using BackEnd.Interfaces;
using BackEnd.Services;
using Microsoft.Extensions.Configuration;


namespace BackEnd.Test
{
    public class ReservationControllerTests
    {
        private ReservationController _controller;
        private IReservationService _reservationService;
        private ApplicationDbContext _context;
        private IEmailService _emailService;

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase("TestDatabase")
        .Options;

            _context = new ApplicationDbContext(options);
            _emailService = new EmailService();
            _reservationService = new ReservationService(_context, _emailService);

            _controller = new ReservationController(_reservationService, configuration);

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
        public async Task GetAllActiveReservationsByUserId_ReturnsNotFound_WhenNoActiveReservations()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = await _controller.GetAllActiveReservationsByUserId(userId);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("No active reservations found for the specified user."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllActiveReservationsByUserId_ReturnsReservations_WhenAreActiveReservations()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "outro sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            var user2 = new UserModel { UserId = 2, FirstName = "John", LastName = "NotDoe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "exampleother@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);
            _context.Users.Add(user);
            _context.Users.Add(user2);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };
            var reservation2 = new Reservation
            {
                opportunityId = opportunity2.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };
            await _controller.CreateEntity(reservation);
            await _controller.CreateEntity(reservation2);

            // Act

            var result = await _controller.GetAllActiveReservationsByUserId(user2.UserId);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var OkResult = result as OkObjectResult;
            Assert.That(OkResult, Is.Not.Null);

            var returnedList = OkResult.Value as IEnumerable<Reservation>;
            Assert.That(returnedList.Count, Is.EqualTo(2));
        }


        [Test]
        [Category("UnitTest")]
        public async Task GetAllActiveReservationsByUserId_ReturnsNotFound_WhenDBContextMissing()
        {
            // Arrange
            var  emailService = new EmailService();
            var reservationService = new ReservationService(null, emailService);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();
            var controller = new ReservationController(reservationService, configuration);

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "outro sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            var user2 = new UserModel { UserId = 2, FirstName = "John", LastName = "NotDoe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "exampleother@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);
            _context.Users.Add(user);
            _context.Users.Add(user2);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };
            var reservation2 = new Reservation
            {
                opportunityId = opportunity2.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };
            await _controller.CreateEntity(reservation);
            await _controller.CreateEntity(reservation2);

            // Act

            var result = await controller.GetAllActiveReservationsByUserId(user2.UserId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing."));
        }


        [Test]
        [Category("UnitTest")]
        public async Task GetAllReservationsByUserId_ReturnsReservations_WhenAreReservations()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "outro sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            var user2 = new UserModel { UserId = 2, FirstName = "John", LastName = "NotDoe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "exampleother@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);
            _context.Users.Add(user);
            _context.Users.Add(user2);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };
            var reservation2 = new Reservation
            {
                opportunityId = opportunity2.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = false,
                fixedPrice = 100
            };
            await _controller.CreateEntity(reservation);
            await _controller.CreateEntity(reservation2);

            // Act

            var result = await _controller.GetAllReservationByUserId(user2.UserId);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var OkResult = result as OkObjectResult;
            Assert.That(OkResult, Is.Not.Null);

            var returnedList = OkResult.Value as IEnumerable<Reservation>;
            Assert.That(returnedList.Count, Is.EqualTo(2));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllReservationsByUserId_ReturnsNotFound_WhenDBContextMissing()
        {
            // Arrange
            var emailService = new EmailService();
            var reservationService = new ReservationService(null, emailService);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();
            var controller = new ReservationController(reservationService, configuration);

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "outro sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            var user2 = new UserModel { UserId = 2, FirstName = "John", LastName = "NotDoe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "exampleother@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);
            _context.Users.Add(user);
            _context.Users.Add(user2);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };
            var reservation2 = new Reservation
            {
                opportunityId = opportunity2.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = false,
                fixedPrice = 100
            };
            await _controller.CreateEntity(reservation);
            await _controller.CreateEntity(reservation2);

            // Act

            var result = await controller.GetAllReservationByUserId(user2.UserId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllReservationsByUserId_ReturnsNotFound_WhenNoReservations()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = await _controller.GetAllReservationByUserId(userId);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("No reservations found for the specified user."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetReservationById_ReturnsNotFound_WhenNoReservation()
        {
            // Arrange
            int ReservationID = 1;

            // Act
            var result = await _controller.GetEntityById(ReservationID);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("Reservation not found."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetReservationById_ReturnsReservation_WhenReservation()
        {
            // Arrange

            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                reservationDate = DateTime.Now.Date,
                Date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                IsActive = true,
                fixedPrice = 100
            };
            _context.Reservations.Add(reservation);

            // Act
            var result = await _controller.GetEntityById(reservation.reservationID);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var returnedReservation = okResult.Value as BackEnd.Models.FrontEndModels.Reservation;
            Assert.That(returnedReservation, Is.Not.Null);
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetReservationById_ReturnsNotFound_WhenDBContextMissing()
        {
            // Arrange
            var emailService = new EmailService();
            var reservationService = new ReservationService(null, emailService);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();
            var controller = new ReservationController(reservationService, configuration);

            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                reservationDate = DateTime.Now.Date,
                Date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                IsActive = true,
                fixedPrice = 100
            };
            _context.Reservations.Add(reservation);

            // Act
            var result = await controller.GetEntityById(reservation.reservationID);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewReservation_ReturnsCreated_WhenReservationIsValid()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address= "um sitio", Category= Enums.Category.AGRICULTURA, UserID= 1,Name="name", Description="a description", Date= DateTime.Now.AddDays(30),Vacancies=2,IsActive=true,Location= Enums.Location.LISBOA,Score= 0, IsImpulsed=false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = new byte[]{ 0xFF, 0xD8, 0xFF, 0xE0 } };

            _context.Opportunities.Add(opportunity);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };

            // Act
            var result = await _controller.CreateEntity(reservation);

            // Assert
            Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(201, Is.EqualTo(createdResult.StatusCode));

            // Comparando as propriedades individualmente
            var createdReservation = createdResult.Value as Reservation;
            Assert.That(createdReservation.opportunityId, Is.EqualTo(reservation.opportunityId));
            Assert.That(createdReservation.userId, Is.EqualTo(reservation.userId));
            Assert.That(createdReservation.reservationDate, Is.EqualTo(reservation.reservationDate));
            Assert.That(createdReservation.date, Is.EqualTo(reservation.date));
            Assert.That(createdReservation.numOfPeople, Is.EqualTo(reservation.numOfPeople));
            Assert.That(createdReservation.isActive, Is.EqualTo(reservation.isActive));
            Assert.That(createdReservation.fixedPrice, Is.EqualTo(reservation.fixedPrice));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewReservation_ReturnsNotFound_WhenDBContextMissing()
        {
            // Arrange
            var emailService = new EmailService();
            var reservationService = new ReservationService(null, emailService);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();
            var controller = new ReservationController(reservationService, configuration);

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 } };

            _context.Opportunities.Add(opportunity);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };

            // Act
            var result = await controller.CreateEntity(reservation);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewReservation_ReturnsBadRequest_WhenReservationIsInvalid()
        {
            // Arrange

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Opportunities.Add(opportunity);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = 0,
                userId = 0,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };

            // Act
            var result = await _controller.CreateEntity(reservation);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            var createdResult = result as BadRequestObjectResult;
            Assert.That(createdResult.Value, Is.EqualTo("Some required fields are missing or invalid."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewReservation_ReturnsNotFound_WhenOpportunityIsInvalid()
        {
            // Arrange

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Opportunities.Add(opportunity);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = 2,
                userId = user.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };

            // Act
            var result = await _controller.CreateEntity(reservation);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            var createdResult = result as NotFoundObjectResult;
            Assert.That(createdResult.Value, Is.EqualTo("Opportunity not found."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewReservation_ReturnsNotFound_WhenUserIsInvalid()
        {
            // Arrange

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Opportunities.Add(opportunity);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = 4,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };

            // Act
            var result = await _controller.CreateEntity(reservation);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            var createdResult = result as NotFoundObjectResult;
            Assert.That(createdResult.Value, Is.EqualTo("User not found."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewReservation_ReturnsBadRequest_WhenNumOfPeopleIsInvalid()
        {
            // Arrange

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 } };

            _context.Opportunities.Add(opportunity);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = -1,
                isActive = true,
                fixedPrice = 100
            };

            // Act
            var result = await _controller.CreateEntity(reservation);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            var createdResult = result as BadRequestObjectResult;
            Assert.That(createdResult.Value, Is.EqualTo("The value Number Of People must be valid"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewReservation_ReturnsBadRequest_WhenNumOfPeopleIsGreaterThanVacancies()
        {
            // Arrange

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Opportunities.Add(opportunity);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user.UserId,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 3,
                isActive = true,
                fixedPrice = 100
            };

            // Act
            var result = await _controller.CreateEntity(reservation);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var createdResult = result as BadRequestObjectResult;
            Assert.That(createdResult.Value, Is.EqualTo("The numberOfPeople is bigger than number of vacancies"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeactivateReservationById_ReturnsOk_WhenExistReservation()
        {
            // Arrange

            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                reservationDate = DateTime.Now.AddDays(-10),
                Date = DateTime.Now.AddMonths(2),
                numOfPeople = 1,
                IsActive = true,
                fixedPrice = 100
            };
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeactivateReservationById(reservation.reservationID);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

        }

        [Test]
        [Category("UnitTest")]
        public async Task DeactivateReservationById_ReturnsNotFound_WhenDBContextMissing()
        {
            // Arrange
            var emailService = new EmailService();
            var reservationService = new ReservationService(null, emailService);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();
            var controller = new ReservationController(reservationService, configuration);

            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                reservationDate = DateTime.Now.AddDays(-10),
                Date = DateTime.Now.AddMonths(2),
                numOfPeople = 1,
                IsActive = true,
                fixedPrice = 100
            };
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            // Act
            var result = await controller.DeactivateReservationById(reservation.reservationID);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing."));

        }

        [Test]
        [Category("UnitTest")]
        public async Task DeactivateReservationById_ReturnsNotFound_WhenDoesntExistReservation()
        {
            // Arrange


            var reservationID = 1;
            

            // Act
            var result = await _controller.DeactivateReservationById(reservationID);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo($"Reservation with id {reservationID} not found."));

        }

        [Test]
        [Category("UnitTest")]
        public async Task DeactivateReservationById_ReturnsBadRequest_WhenReservationAllreadyInnactive()
        {
            // Arrange

            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                reservationDate = DateTime.Now.Date,
                Date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                IsActive = false,
                fixedPrice = 100
            };
            _context.Reservations.Add(reservation);

            // Act
            var result = await _controller.DeactivateReservationById(reservation.reservationID);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            var BadResult = result as BadRequestObjectResult;
            Assert.That(BadResult.Value, Is.EqualTo("Reservation is impossible to deactivate"));

        }

        [Test]
        [Category("UnitTest")]
        public async Task UpdateReservationById_ReturnsOk_WhenReservationUpdates()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 5, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            _context.Opportunities.Add(opportunity);
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Users.Add(user);
            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                reservationDate = DateTime.Now.Date,
                Date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                IsActive = false,
                fixedPrice = 100
            };
            _context.Reservations.Add(reservation);
            var reservationDTO = new Reservation
            {
                opportunityId = 1,
                userId = 1,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 3,
                isActive = false,
                fixedPrice = 150
            };
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.UpdateEntity(reservation.reservationID, reservationDTO);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            
        }

        [Test]
        [Category("UnitTest")]
        public async Task UpdateReservationById_ReturnsNotFound_WhenDBContextMissing()
        {
            // Arrange
            var emailService = new EmailService();
            var reservationService = new ReservationService(null, emailService);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();
            var controller = new ReservationController(reservationService, configuration);

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 5, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            _context.Opportunities.Add(opportunity);
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Users.Add(user);
            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                reservationDate = DateTime.Now.Date,
                Date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                IsActive = false,
                fixedPrice = 100
            };
            _context.Reservations.Add(reservation);
            var reservationDTO = new Reservation
            {
                opportunityId = 1,
                userId = 1,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 3,
                isActive = false,
                fixedPrice = 150
            };
            await _context.SaveChangesAsync();

            // Act
            var result = await controller.UpdateEntity(reservation.reservationID, reservationDTO);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing."));
        }


        [Test]
        [Category("UnitTest")]
        public async Task UpdateReservationById_ReturnsNotFound_WhenReservationDoesntExist()
        {
            // Arrange

            var reservationid = 1;
            var reservationDTO = new Reservation
            {
                opportunityId = 1,
                userId = 1,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 3,
                isActive = false,
                fixedPrice = 150
            };

            // Act
            var result = await _controller.UpdateEntity(reservationid, reservationDTO);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var NotFoundResult = result as NotFoundObjectResult;
            Assert.That(NotFoundResult.Value, Is.EqualTo("Reservation not found."));

        }

        [Test]
        [Category("UnitTest")]
        public async Task UpdateReservationById_ReturnsBadRequest_WhenInvalidNumOfPeople()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 5, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            _context.Opportunities.Add(opportunity);
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Users.Add(user);
            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                reservationDate = DateTime.Now.Date,
                Date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                IsActive = false,
                fixedPrice = 100
            };
            _context.Reservations.Add(reservation);
            var reservationDTO = new Reservation
            {
                opportunityId = 1,
                userId = 1,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = -3,
                isActive = false,
                fixedPrice = 150
            };
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.UpdateEntity(reservation.reservationID, reservationDTO);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            var BadResult = result as BadRequestObjectResult;
            Assert.That(BadResult.Value, Is.EqualTo("The value Number Of People must be valid."));


        }

        [Test]
        [Category("UnitTest")]
        public async Task UpdateReservationById_ReturnsBadRequest_WhenNumOfPeopleGreaterthanVacancies()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 5, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            _context.Opportunities.Add(opportunity);
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _context.Users.Add(user);
            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                reservationDate = DateTime.Now.Date,
                Date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                IsActive = false,
                fixedPrice = 100
            };
            _context.Reservations.Add(reservation);
            var reservationDTO = new Reservation
            {
                opportunityId = 1,
                userId = 1,
                reservationDate = DateTime.Now.Date,
                date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 7,
                isActive = false,
                fixedPrice = 150
            };
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.UpdateEntity(reservation.reservationID, reservationDTO);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            var BadResult = result as BadRequestObjectResult;
            Assert.That(BadResult.Value, Is.EqualTo("The numberOfPeople is bigger than number of vacancies."));


        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteReservation_ReturnsOK_WhenReservationExists()
        {
            // Arrange
            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                Date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                fixedPrice = 100,
                IsActive = true
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteEntity(reservation.reservationID);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That(await _context.Reservations.FindAsync(reservation.reservationID), Is.Null);
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteReservation_ReturnsNotFound_WhenDBContextMissing()
        {
            // Arrange
            var emailService = new EmailService();
            var reservationService = new ReservationService(null, emailService);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();
            var controller = new ReservationController(reservationService, configuration);

            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                Date = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                fixedPrice = 100,
                IsActive = true
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Act
            var result = await controller.DeleteEntity(reservation.reservationID);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing."));
        }


        [Test]
        [Category("UnitTest")]
        public async Task DeleteReservation_ReturnsNotfound_WhenReservationDontExists()
        {
            // Arrange
            var reservationid = 1;
  

            // Act
            var result = await _controller.DeleteEntity(reservationid);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            
        }
    }
}
