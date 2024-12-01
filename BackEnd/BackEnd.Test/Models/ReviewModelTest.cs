using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;

namespace BackEnd.Tests.Models
{
    [TestFixture]
    public class ReviewModelTest
    {
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            // Setup DbContext InMemory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup database after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        public ReviewModel CreateReviewExample(int reservationId, string description, float rating)
        {
            return new ReviewModel
            {
                ReservationId = reservationId,
                Desc = description,
                Rating = rating
            };
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateReviewModel_ShouldCreateSuccessfully()
        {
            // Arrange
            var reviewModel = CreateReviewExample(1, "Muito Bem", 3.5f);

            // Act
            await _context.Reviews.AddAsync(reviewModel);
            await _context.SaveChangesAsync();

            // Asserts
            var reviews = await _context.Reviews.ToListAsync();
            Assert.That(reviews.Count, Is.EqualTo(1));
            Assert.That(reviews.First().Desc, Is.EqualTo("Muito Bem"));
            Assert.That(reviews.First().Rating, Is.EqualTo(3.5f));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetReviewById_ShouldReturnReview()
        {
            // Arrange
            var reviewModel = CreateReviewExample(1, "Muito Bem", 3.5f);
            await _context.Reviews.AddAsync(reviewModel);
            await _context.SaveChangesAsync();

            // Act
            var retrievedReview = await _context.Reviews.FindAsync(reviewModel.ReservationId);

            // Assert
            Assert.That(retrievedReview, Is.Not.Null);
            Assert.That(retrievedReview.Desc, Is.EqualTo("Muito Bem"));
            Assert.That(retrievedReview.Rating, Is.EqualTo(3.5f));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditReviewById_ShouldEditRatingFromReview()
        {
            // Arrange
            var reviewModel = CreateReviewExample(1, "Muito Bem", 3.5f);
            await _context.Reviews.AddAsync(reviewModel);
            await _context.SaveChangesAsync();

            // Act
            var retrievedReview = await _context.Reviews.FindAsync(reviewModel.ReservationId);
            retrievedReview.Rating = 5.0f;
            await _context.SaveChangesAsync();

            // Assert
            var updatedReview = await _context.Reviews.FindAsync(reviewModel.ReservationId);
            Assert.That(updatedReview.Rating, Is.EqualTo(5.0f));
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteReviewById_ShouldDeleteReviewFromDatabase()
        {
            // Arrange
            var reviewModel = CreateReviewExample(1, "Muito Bem", 3.5f);
            await _context.Reviews.AddAsync(reviewModel);
            await _context.SaveChangesAsync();

            // Act
            _context.Remove(reviewModel);
            await _context.SaveChangesAsync();

            // Assert
            var reviews = await _context.Reviews.ToListAsync();
            Assert.That(reviews.Count, Is.EqualTo(0));
        }
    }
}
