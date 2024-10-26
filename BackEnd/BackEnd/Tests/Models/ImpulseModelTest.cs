using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BackEnd.Tests.Models
{
    [TestFixture]
    public class ImpulseModelTest
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

        private ImpulseModel CreateImpulseExample(int UserId, int OpportunityId, decimal Price, DateTime ExpDate)
        {
            var user = new UserModel
            {
                HashedPassword = "jadjkahdfjahf",
                FirstName = "Manel",
                LastName = "Antonio",
                Email = "example@gmail.com",
                CellPhoneNum = 912345678,
                BirthDate = DateTime.Now,
                Gender = Gender.MASCULINO,
                Token = "teste12345678"
            };
            var opp = new OpportunityModel
            {
                Name = "teste",
                Description = "testeteste123456789",
                Price = (decimal)14.99,
                Vacancies = 2,
                Category = Category.AGRICULTURA,
                Location = Location.VIANA_DO_CASTELO,
                Address = "RUA 123",
                Score = (float)3.5,
                IsImpulsed = true
            };
            return new ImpulseModel
            {
                UserId = UserId,
                User = user,
                OpportunityId = OpportunityId,
                Opportunity = opp,
                Price = Price,
                ExpireDate = ExpDate
            };
        }

        [Test]
        public async Task CreateImpulseModel_ShouldCreateSuccessfully()
        {
            // Arrange
            var impulseModel = CreateImpulseExample(1, 1, 15.99M, new DateTime(1990, 1, 1).AddDays(30));

            // Act
            await _context.Impulses.AddAsync(impulseModel);
            await _context.SaveChangesAsync();

            // Assert
            var impulses = await _context.Impulses.ToListAsync();
            Assert.That(impulses.Count, Is.EqualTo(1));
            Assert.That(impulses.First().Price, Is.EqualTo(15.99M));
            Assert.That(impulses.First().ExpireDate, Is.EqualTo(new DateTime(1990, 1, 1).AddDays(30)));
        }

        [Test]
        public async Task GetImpulseById_ShouldReturnImpulse()
        {
            // Arrange
            var impulseModel = CreateImpulseExample(1, 1, 15.99M, new DateTime(1990, 1, 1).AddDays(30));
            await _context.Impulses.AddAsync(impulseModel);
            await _context.SaveChangesAsync();

            // Act
            var retrievedImpulse = await _context.Impulses.FindAsync(impulseModel.UserId, impulseModel.OpportunityId);


            // Assert
            Assert.That(retrievedImpulse, Is.Not.Null);
            Assert.That(retrievedImpulse.Price, Is.EqualTo(15.99M));
            Assert.That(retrievedImpulse.ExpireDate, Is.EqualTo(new DateTime(1990, 1, 1).AddDays(30)));
        }

        [Test]
        public async Task EditImpulseById_ShouldEditPriceFromImpulse()
        {
            // Arrange
            var impulseModel = CreateImpulseExample(1, 1, 15.99M, new DateTime(1990, 1, 1).AddDays(30));
            await _context.Impulses.AddAsync(impulseModel);
            await _context.SaveChangesAsync();

            // Act
            var retrievedImpulse = await _context.Impulses.FindAsync(impulseModel.UserId, impulseModel.OpportunityId);
            retrievedImpulse.Price = 20.00M;
            await _context.SaveChangesAsync();

            // Assert
            var updatedImpulse = await _context.Impulses.FindAsync(impulseModel.UserId, impulseModel.OpportunityId);
            Assert.That(updatedImpulse.Price, Is.EqualTo(20.00M));
        }

        [Test]
        public async Task DeleteImpulseById_ShouldDeleteImpulseFromDatabase()
        {
            // Arrange
            var impulseModel = CreateImpulseExample(1, 1, 15.99M, new DateTime(1990, 1, 1).AddDays(30));
            await _context.Impulses.AddAsync(impulseModel);
            await _context.SaveChangesAsync();

            // Act
            _context.Remove(impulseModel);
            await _context.SaveChangesAsync();

            // Assert
            var impulses = await _context.Impulses.ToListAsync();
            Assert.That(impulses.Count, Is.EqualTo(0));
        }
    }
}

