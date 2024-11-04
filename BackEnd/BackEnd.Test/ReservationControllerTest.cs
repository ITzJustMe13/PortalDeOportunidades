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


namespace BackEnd.Tests
{
    public class ReservationControllerTests
    {
        private ReservationController _controller;
        private ApplicationDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _controller = new ReservationController(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup the in-memory database after each test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
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
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("No active reservations found for the specified user."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllActiveReservationsByUserId_ReturnsReservations_WhenAreActiveReservations()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "outro sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            var user2 = new UserModel { UserId = 2, FirstName = "John", LastName = "NotDoe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "exampleother@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _dbContext.Opportunities.Add(opportunity);
            _dbContext.Opportunities.Add(opportunity2);
            _dbContext.Users.Add(user);
            _dbContext.Users.Add(user2);
            await _dbContext.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };
            var reservation2 = new Reservation
            {
                opportunityId = opportunity2.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };
            await _controller.CreateNewReservation(reservation);
            await _controller.CreateNewReservation(reservation2);

            // Act

            var result = await _controller.GetAllActiveReservationsByUserId(user2.UserId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var OkResult = result.Result as OkObjectResult;
            Assert.That(OkResult, Is.Not.Null);

            var returnedList = OkResult.Value as IEnumerable<Reservation>;
            Assert.That(returnedList.Count, Is.EqualTo(2));
        }


        [Test]
        [Category("UnitTest")]
        public async Task GetAllReservationsByUserId_ReturnsReservations_WhenAreReservations()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "outro sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            var user2 = new UserModel { UserId = 2, FirstName = "John", LastName = "NotDoe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "exampleother@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _dbContext.Opportunities.Add(opportunity);
            _dbContext.Opportunities.Add(opportunity2);
            _dbContext.Users.Add(user);
            _dbContext.Users.Add(user2);
            await _dbContext.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = opportunity.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };
            var reservation2 = new Reservation
            {
                opportunityId = opportunity2.OpportunityId,
                userId = user2.UserId,
                reservationDate = DateTime.Now.Date,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = false,
                fixedPrice = 100
            };
            await _controller.CreateNewReservation(reservation);
            await _controller.CreateNewReservation(reservation2);

            // Act

            var result = await _controller.GetAllReservationByUserId(user2.UserId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var OkResult = result.Result as OkObjectResult;
            Assert.That(OkResult, Is.Not.Null);

            var returnedList = OkResult.Value as IEnumerable<Reservation>;
            Assert.That(returnedList.Count, Is.EqualTo(2));
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
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("No reservations found for the specified user."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetReservationById_ReturnsNotFound_WhenNoReservation()
        {
            // Arrange
            int ReservationID = 1;

            // Act
            var result = await _controller.GetReservationById(ReservationID);

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
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
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };
            _dbContext.Reservations.Add(reservation);

            // Act
            var result = await _controller.GetReservationById(reservation.reservationID);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var returnedReservation = okResult.Value as BackEnd.Models.FrontEndModels.Reservation;
            Assert.That(returnedReservation, Is.Not.Null);
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewReservation_ReturnsCreated_WhenReservationIsValid()
        {
            // Arrange

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address= "um sitio", Category= Enums.Category.AGRICULTURA, userID= 1,Name="name", Description="a description", date= DateTime.Now.AddDays(30),Vacancies=2,IsActive=true,Location= Enums.Location.LISBOA,Score= 0, IsImpulsed=false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName="Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email= "example@email.com", Gender=Enums.Gender.MASCULINO, Image= [1] };
            _dbContext.Opportunities.Add(opportunity);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

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
            var result = await _controller.CreateNewReservation(reservation);

            // Assert
            Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(201, Is.EqualTo(createdResult.StatusCode));
            Assert.That(reservation, Is.EqualTo(createdResult.Value));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateNewReservation_ReturnsBadRequest_WhenReservationIsInvalid()
        {
            // Arrange

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _dbContext.Opportunities.Add(opportunity);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var reservation = new Reservation
            {
                opportunityId = 0,
                userId = 0,
                reservationDate = DateTime.Now.Date,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };

            // Act
            var result = await _controller.CreateNewReservation(reservation);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var createdResult = result.Result as BadRequestObjectResult;
            Assert.That(400, Is.EqualTo(createdResult.StatusCode));
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
                checkInDate = DateTime.Now.AddMonths(2),
                numOfPeople = 1,
                isActive = true,
                fixedPrice = 100
            };
            await _dbContext.Reservations.AddAsync(reservation);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.DeactivateReservationById(reservation.reservationID);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkResult>());
            var okResult = result.Result as OkResult;
            Assert.That(okResult, Is.Not.Null);

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
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
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
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = false,
                fixedPrice = 100
            };
            _dbContext.Reservations.Add(reservation);

            // Act
            var result = await _controller.DeactivateReservationById(reservation.reservationID);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var BadResult = result.Result as BadRequestObjectResult;
            Assert.That(400, Is.EqualTo(BadResult.StatusCode));

        }

        [Test]
        [Category("UnitTest")]
        public async Task UpdateReservationById_ReturnsNoContent_WhenReservationUpdates()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 5, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            _dbContext.Opportunities.Add(opportunity);
            var user = new UserModel { UserId = 1, FirstName = "John", LastName = "Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email = "example@email.com", Gender = Enums.Gender.MASCULINO, Image = [1] };
            _dbContext.Users.Add(user);
            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                reservationDate = DateTime.Now.Date,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                isActive = false,
                fixedPrice = 100
            };
            _dbContext.Reservations.Add(reservation);
            var reservationDTO = new Reservation
            {
                opportunityId = 1,
                userId = 1,
                reservationDate = DateTime.Now.Date,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 3,
                isActive = false,
                fixedPrice = 150
            };
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.UpdateReservation(reservation.reservationID, reservationDTO);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            

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
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 3,
                isActive = false,
                fixedPrice = 150
            };

            // Act
            var result = await _controller.UpdateReservation(reservationid, reservationDTO);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());


        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteReservation_ReturnsNoContent_WhenReservationExists()
        {
            // Arrange
            var reservation = new ReservationModel
            {
                reservationID = 1,
                opportunityID = 1,
                userID = 1,
                checkInDate = DateTime.Now.Date.AddDays(1),
                numOfPeople = 1,
                fixedPrice = 100,
                isActive = true
            };
            _dbContext.Reservations.Add(reservation);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteReservation(reservation.reservationID);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            Assert.That(await _dbContext.Reservations.FindAsync(reservation.reservationID), Is.Null);
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteReservation_ReturnsNotfound_WhenReservationDontExists()
        {
            // Arrange
            var reservationid = 1;
  

            // Act
            var result = await _controller.DeleteReservation(reservationid);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            
        }


    }
}
