using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.Models.BackEndModels;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;

namespace BackEnd.Tests.Models

{
    [TestFixture]
    public class UserModelTest
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

        // Example data seeding method (foi a maneira mais simples de fzr)
        private UserModel CreateSampleUser(string HashedPassword, int? ExternalId, string FirstName, string LastName, string Email, int CellPhoneNum, DateTime BirthDate, Gender Gender, string Token, DateTime? TokenExpDate)
        {
            return new UserModel
            {
                HashedPassword = HashedPassword,
                ExternalId = ExternalId,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                CellPhoneNum = CellPhoneNum,
                BirthDate = BirthDate,
                Gender = Gender,
                Token = Token,
                TokenExpDate = TokenExpDate
            };
        }

        [Test]
        public async Task AddUser_ShouldAddUserToDatabase()
        {
            // Arrange
            var user = CreateSampleUser("samplehashedpassword",null,"John","Doe","johnDoe@gmail.com",911632142,new DateTime(1990, 1, 1),Gender.MASCULINO,"sampleToken",DateTime.Today.AddDays(1));

            // Act
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.That(users.Count,Is.EqualTo(1));
            Assert.That(users.First().FirstName, Is.EqualTo("John"));
            Assert.That(users.First().LastName, Is.EqualTo("Doe"));
        }

        [Test]
        public async Task GetUserById_ShouldReturnUser()
        {
            // Arrange
            var user = CreateSampleUser("samplehashedpassword", null, "John", "Doe", "johnDoe@gmail.com", 911632142, new DateTime(1990, 1, 1), Gender.MASCULINO, "sampleToken", DateTime.Today.AddDays(1));
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var retrievedUser = await _context.Users.FindAsync(user.UserId);

            // Assert
            Assert.That(retrievedUser, Is.Not.Null);
            Assert.That(retrievedUser.Email, Is.EqualTo("johnDoe@example.com"));
        }

        [Test]
        public async Task UpdateUser_ShouldModifyUserDetails()
        {
            // Arrange
            var user = CreateSampleUser("samplehashedpassword", null, "John", "Doe", "johnDoe@gmail.com", 911632142, new DateTime(1990, 1, 1), Gender.MASCULINO, "sampleToken", DateTime.Today.AddDays(1));
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var retrievedUser = await _context.Users.FindAsync(user.UserId);
            retrievedUser.FirstName = "Jane";
            retrievedUser.LastName = "Smith";
            await _context.SaveChangesAsync();

            // Assert
            var updatedUser = await _context.Users.FindAsync(user.UserId);
            Assert.That(updatedUser.FirstName, Is.EqualTo("Jane"));
            Assert.That(updatedUser.LastName, Is.EqualTo("Smith"));
        }

        [Test]
        public async Task DeleteUser_ShouldRemoveUserFromDatabase()
        {
            // Arrange
            var user = CreateSampleUser("samplehashedpassword", null, "John", "Doe", "johnDoe@gmail.com", 911632142, new DateTime(1990, 1, 1), Gender.MASCULINO, "sampleToken", DateTime.Today.AddDays(1));
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // Assert
            var users = await _context.Users.ToListAsync();
            Assert.That(users.Count, Is.EqualTo(0));
        }
    }
}
