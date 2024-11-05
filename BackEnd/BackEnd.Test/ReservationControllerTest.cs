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


namespace BackEnd.Test
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
        public async Task GetAllActiveReservationsByUserId_ReturnsNotFound_WhenNoActiveReservations()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = await _controller.GetAllActiveReservationsByUserId(userId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.Equals("No active reservations found for the specified user.", notFoundResult.Value);
        }

        [Test]
        public async Task CreateNewReservation_ReturnsCreated_WhenReservationIsValid()
        {
            // Arrange

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address= "um sitio", Category= Enums.Category.AGRICULTURA, userID= 1,Name="name", Description="a description", date= DateTime.Now.AddDays(30),Vacancies=2,IsActive=true,Location= Enums.Location.LISBOA,Score= 0, IsImpulsed=false };
            var user = new UserModel { UserId = 1, FirstName = "John", LastName="Doe", BirthDate = DateTime.Now.AddYears(-30), CellPhoneNum = 919919919, Email= "example@email.com", Gender=Enums.Gender.MASCULINO };
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
            Assert.Equals(201, createdResult.StatusCode);
            Assert.Equals(reservation, createdResult.Value);
        }

        [Test]
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

        // Additional tests can be added here to cover more methods and edge cases.
    }
}
