using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Controllers;
using BackEnd.Controllers.Data;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BackEnd.Test
{
    [TestFixture]
    public class ReviewControllerTests
    {
        private ReviewController _controller;
        private ApplicationDbContext _context; 

        [SetUp]
        public void Setup()
        {
            // Use in-memory database for ApplicationDbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new ReviewController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetReviewById_ReturnsReviewDto_ForValidId()
        {
            // Arrange
            var reviewId = 1;
            var reviewModel = new ReviewModel { ReservationId = reviewId, Rating = 4.5f, Desc = "Test review" };
            _context.Reviews.Add(reviewModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetReviewById(reviewId);

            // Assert
            Assert.That(response.Result, Is.TypeOf<OkObjectResult>(), "Expected OkObjectResult for valid review ID");

            var okResult = response.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "OkObjectResult should not be null");

            var returnedReview = okResult.Value as BackEnd.Models.FrontEndModels.Review;
            Assert.That(returnedReview, Is.Not.Null, "Returned review should be of type ReviewDto");
            Assert.That(returnedReview.reservationId, Is.EqualTo(reviewId), "ReservationId should match the input ID");
            Assert.That(returnedReview.rating, Is.EqualTo(4.5f), "Rating should match the expected value");
            Assert.That(returnedReview.desc, Is.EqualTo("Test review"), "Description should match the expected value");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetReviewById_ReturnsNotFound_ForNonexistentId()
        {
            // Arrange
            var reviewId = 1;

            // Act
            var response = await _controller.GetReviewById(reviewId);

            // Assert
            Assert.That(response.Result, Is.TypeOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult for nonexistent review ID");

            var notFoundResult = response.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null, "NotFoundObjectResult should not be null");
            Assert.That(notFoundResult?.Value, Is.EqualTo($"Review with id {reviewId} not found."), "Error message should match the expected not found message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetReviewById_ReturnsNotFoundObjectResult_DBContextMissing()
        {
            // Arrange
            var controller = new ReviewController(null);
            var reviewId = 1;

            // Act
            var response = await controller.GetReviewById(reviewId);

            // Assert
            Assert.That(response.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response.Result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateReview_ReturnsBadRequest_ForInvalidReservationId()
        {
            // Arrange
            var nonExistentReservationId = 2;

            // Act
            var reviewDto = new BackEnd.Models.FrontEndModels.Review
            {
                reservationId = nonExistentReservationId,
                rating = 4.5f,
                desc = "test review"
            };
            var response = await _controller.CreateReview(reviewDto);

            // Assert
            Assert.That(response.Result, Is.TypeOf<BadRequestObjectResult>());

            var badRequestResult = response.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid Reservation ID. Reservation does not exist."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateReview_ReturnsCreatedAtAction_ForValidReview()
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

            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();

            var reservationModel = new ReservationModel
            {
                reservationID = 1,
                reservationDate = DateTime.Now,
                checkInDate = DateTime.Now,
                opportunityID = 1,
                userID = 1,
                numOfPeople = 2,
                isActive = true
            };

            _context.Reservations.Add(reservationModel);
            await _context.SaveChangesAsync();

            var newReviewDto = new BackEnd.Models.FrontEndModels.Review
            {
                reservationId = reservationModel.reservationID,
                rating = 4.5f,
                desc = "New test review"
            };

            // Act
            var response = await _controller.CreateReview(newReviewDto);

            // Assert
   
            Assert.That(response.Result, Is.TypeOf<CreatedAtActionResult>());
            var createdAtResult = response.Result as CreatedAtActionResult;
            Assert.That(createdAtResult, Is.Not.Null);
            Assert.That(createdAtResult?.ActionName, Is.EqualTo(nameof(ReviewController.GetReviewById)));

            var returnedReview = await _context.Reviews.FindAsync(reservationModel.reservationID);
            Assert.That(returnedReview, Is.Not.Null);
            Assert.That(returnedReview?.ReservationId, Is.EqualTo(newReviewDto.reservationId));
            Assert.That(returnedReview?.Desc, Is.EqualTo("New test review"));
            Assert.That(returnedReview?.Rating, Is.EqualTo(4.5f));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateReview_ReturnsNotFoundt_DBContextMissing()
        {
            // Arrange
            var controller = new ReviewController(null);
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

            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();

            var reservationModel = new ReservationModel
            {
                reservationID = 1,
                reservationDate = DateTime.Now,
                checkInDate = DateTime.Now,
                opportunityID = 1,
                userID = 1,
                numOfPeople = 2,
                isActive = true
            };

            _context.Reservations.Add(reservationModel);
            await _context.SaveChangesAsync();

            var newReviewDto = new BackEnd.Models.FrontEndModels.Review
            {
                reservationId = reservationModel.reservationID,
                rating = 4.5f,
                desc = "New test review"
            };

            // Act
            var response = await controller.CreateReview(newReviewDto);

            // Assert

            Assert.That(response.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response.Result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing"));
        }


        [Test]
        [Category("UnitTest")]
        public async Task CreateReview_ReturnsBadRequest_ForDuplicateReview()
        {
            // Arrange
            var validReservationId = 1;

            var reservationModel = new ReservationModel
            {
                reservationID = validReservationId,
                reservationDate = DateTime.Now,
                checkInDate = DateTime.Now,
                opportunityID = 1,
                userID = 1,
                numOfPeople = 2,
                isActive = true
            };

            _context.Reservations.Add(reservationModel);
            await _context.SaveChangesAsync();

            var existingReviewDto = new BackEnd.Models.FrontEndModels.Review
            {
                reservationId = validReservationId,
                rating = 4.0f,
                desc = "Existing test review"
            };
            await _controller.CreateReview(existingReviewDto);

            // Create a new review DTO with the same reservation ID
            var duplicateReviewDto = new BackEnd.Models.FrontEndModels.Review
            {
                reservationId = validReservationId,
                rating = 5.0f,
                desc = "Duplicate test review"
            };

            // Act
            var response = await _controller.CreateReview(duplicateReviewDto);

            // Assert
            Assert.That(response.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("A review for this reservation already exists."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateReview_ReturnsBadRequest_ForScoreBelow0OrBiggerThan5()
        {
            // Arrange
            var reservationId = 1;
            var reviewDto = new BackEnd.Models.FrontEndModels.Review
            {
                reservationId = reservationId,
                rating = 6.5f,
                desc = "test review"
            };

            // Act

            var response = await _controller.CreateReview(reviewDto);

            // Assert
            Assert.That(response.Result, Is.TypeOf<BadRequestObjectResult>());

            var badRequestResult = response.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Rating can't be below 0 or bigger than 5"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateReview_ReturnsBadRequest_ForNullReview()
        {
            // Arrange
            Models.FrontEndModels.Review reviewDto = null;

            // Act
            var response = await _controller.CreateReview(reviewDto);

            // Assert
            Assert.That(response.Result, Is.TypeOf<BadRequestObjectResult>());

            var badRequestResult = response.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Review data is required."));
        }


        [Test]
        [Category("UnitTest")]
        public async Task DeleteReview_ReturnsNotFound_ForInvalidReservationId()
        {
            // Arrange
            var reservationId = 1;
            var invalidReservationId = 2;
            var reviewModel = new ReviewModel { ReservationId = reservationId, Rating = 4.5f, Desc = "Test review" };
            _context.Reviews.Add(reviewModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.DeleteReviewById(invalidReservationId);

            // Assert
            Assert.That(response.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = response.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult?.Value, Is.EqualTo($"Review with id {invalidReservationId} not found."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteReview_ReturnsNoContent_ForValidReservationId()
        {
            // Arrange
            var reservationId = 1;
            var reviewModel = new ReviewModel { ReservationId = reservationId, Rating = 4.5f, Desc = "Test review" };
            _context.Reviews.Add(reviewModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.DeleteReviewById(reservationId);

            // Assert
            Assert.That(response.Result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteReview_ReturnsNotFound_ForDBContextMissing()
        {
            // Arrange
            var controller = new ReviewController(null);
            var reservationId = 1;
            var reviewModel = new ReviewModel { ReservationId = reservationId, Rating = 4.5f, Desc = "Test review" };
            _context.Reviews.Add(reviewModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await controller.DeleteReviewById(reservationId);

            // Assert
            Assert.That(response.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response.Result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditReview_ReturnsBadRequest_ForInvalidReservationId()
        {
            // Arrange
            var reservationId = 1;
            var invalidReservationId = 2;
            var reviewModel = new ReviewModel { ReservationId = reservationId, Rating = 4.5f, Desc = "Test review" };
            _context.Reviews.Add(reviewModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.EditReviewById(2, 4.5F, "Great test");

            // Assert
            Assert.That(response.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = response.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult?.Value, Is.EqualTo($"Review with id {invalidReservationId} not found."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditReview_ReturnsOK_ForValidReservationId()
        {
            // Arrange
            var reservationId = 1;
            var reviewModel = new ReviewModel { ReservationId = reservationId, Rating = 4.5f, Desc = "Test review" };
            _context.Reviews.Add(reviewModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.EditReviewById(1, 2.5F, "Great test");

            // Assert
            Assert.That(response.Result, Is.TypeOf<OkObjectResult>());
            var okResult = response.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var returnedReview = okResult.Value as BackEnd.Models.FrontEndModels.Review;
            Assert.That(returnedReview, Is.Not.Null);
            Assert.That(returnedReview.reservationId, Is.EqualTo(reservationId));
            Assert.That(returnedReview.rating, Is.EqualTo(2.5f));
            Assert.That(returnedReview.desc, Is.EqualTo("Great test"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditReview_ReturnsBadRequest_ForScoreBelow0OrBiggerThan5()
        {
            // Arrange
            var reservationId = 1;
            var reviewModel = new ReviewModel { ReservationId = reservationId, Rating = 4.5f, Desc = "Test review" };
            _context.Reviews.Add(reviewModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.EditReviewById(1, -1, "Great test");

            // Assert
            Assert.That(response.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = response.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult?.Value, Is.EqualTo("Rating can't be below 0 or bigger than 5"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditReview_ReturnsNotFound_ForDBContextMissing()
        {
            // Arrange
            var controller = new ReviewController(null);
            var reservationId = 1;
            var reviewModel = new ReviewModel { ReservationId = reservationId, Rating = 4.5f, Desc = "Test review" };
            _context.Reviews.Add(reviewModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await controller.EditReviewById(1, 2.5F, "Great test");

            // Assert
            Assert.That(response.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response.Result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing"));
        }
    }
}
