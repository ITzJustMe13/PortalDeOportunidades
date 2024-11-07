using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.Models.BackEndModels;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BackEnd.Tests.Models
{
    [TestFixture]
    public class OpportunityModelTest
    {

        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("Filename=:memory:")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.OpenConnection(); 
            _context.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.CloseConnection();
            _context.Dispose();
        }

        private OpportunityModel CreateSampleOpportunity(UserModel User,int userID,string Name, string Description, decimal Price, int Vacancies, bool IsActive, Category Category, Location Location, string Address, float Score, bool IsImpulsed)
        {
            return new OpportunityModel
            {
                UserID = userID,
                User = User,
                Name = Name,
                Description = Description,
                Price = Price,
                Vacancies = Vacancies,
                IsActive = IsActive,
                Category = Category,
                Location = Location,
                Address = Address,
                Score = Score,
                IsImpulsed = IsImpulsed
            };
        }

        private UserModel CreateSampleUser()
        {
            return new UserModel
            {
                HashedPassword = "exampleHas",
                FirstName = "toni",
                LastName = "naotoni",
                Email = "toninaotoni@gmail.com",
                CellPhoneNum = 919191999,
                RegistrationDate = DateTime.Now,
                BirthDate = new DateTime(1998,11,12),
                Gender = Gender.MASCULINO,
                Token = "Token",
                TokenExpDate = DateTime.Now.AddDays(1),
                Image = [1]
            };
        }

        private OpportunityModel CreateFalseSampleOpp()
        {
            return new OpportunityModel
            {
                Name = null,
                Description = null,
                Price = (decimal)23.12,
                Vacancies = 30,
                IsActive = true,
                Category = Category.AGRICULTURA,
                Location = Location.LISBOA,
                Address = "Uma rua de lisboa nº890",
                Score = 3.3F,
                IsImpulsed = true
            };
        }

        [Test]
        [Category("UnitTest")]
        public async Task AddOpp_ShouldNotAddOppToDatabase()
        {
            // Arrange
            var opp = CreateFalseSampleOpp();

            // Act
            await _context.Opportunities.AddAsync(opp);
            var exception = Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await _context.SaveChangesAsync(); 
            });

            Assert.That(exception.Message, Does.Contain("An error occurred while saving the entity changes. See the inner exception for details."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task AddOpp_ShouldAddOppToDatabase()
        {
            // Arrange
            var user = CreateSampleUser();
            var opp = CreateSampleOpportunity(user, user.UserId, "TestOportunidade", "Uma oportunidade de teste", (decimal)21.32, 30, true, Category.ARTESANATO, Location.FARO, "um sitio em faro", 3.2F, true);

            // Act
            await _context.Users.AddAsync(user);
            await _context.Opportunities.AddAsync(opp);
            await _context.SaveChangesAsync();

            // Assert
            var opps = await _context.Opportunities.ToListAsync();
            Assert.That(opps.Count, Is.EqualTo(1));
            Assert.That(opps.First().Name, Is.EqualTo("TestOportunidade"));
            Assert.That(opps.First().Description, Is.EqualTo("Uma oportunidade de teste"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetOppById_ShouldReturnOpp()
        {
            // Arrange
            
            var user = CreateSampleUser();
            var opp = CreateSampleOpportunity(user, user.UserId, "TestOportunidade", "Uma oportunidade de teste", (decimal)21.32, 30, true, Category.ARTESANATO, Location.FARO, "um sitio em faro", 3.2F, true);
            await _context.Users.AddAsync(user);
            await _context.Opportunities.AddAsync(opp);
            await _context.SaveChangesAsync();

            // Act
            var retrievedOpp = await _context.Opportunities.FindAsync(opp.OpportunityId);

            // Assert
            Assert.That(retrievedOpp, Is.Not.Null);
        }

        [Test]
        [Category("UnitTest")]
        public async Task UpdateOpp_ShouldModifyOppDetails()
        {
            // Arrange
            var user = CreateSampleUser();
            var opp = CreateSampleOpportunity(user, user.UserId, "TestOportunidade", "Uma oportunidade de teste", (decimal)21.32, 30, true, Category.ARTESANATO, Location.FARO, "um sitio em faro", 3.2F, true);
            await _context.Users.AddAsync(user);
            await _context.Opportunities.AddAsync(opp);
            await _context.SaveChangesAsync();

            // Act
            var retrievedOpp = await _context.Opportunities.FindAsync(opp.OpportunityId);
            retrievedOpp.Description = "SECOND DESC";
            await _context.SaveChangesAsync();

            // Assert
            var updatedUser = await _context.Opportunities.FindAsync(opp.OpportunityId);
            Assert.That(updatedUser.Description, Is.EqualTo("SECOND DESC"));
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteOpp_ShouldRemoveOppFromDatabase()
        {
            // Arrange
            var user = CreateSampleUser();
            var opp = CreateSampleOpportunity(user, user.UserId, "TestOportunidade", "Uma oportunidade de teste", (decimal)21.32, 30, true, Category.ARTESANATO, Location.FARO, "um sitio em faro", 3.2F, true);
            await _context.Users.AddAsync(user);
            await _context.Opportunities.AddAsync(opp);
            await _context.SaveChangesAsync();

            // Act
            _context.Opportunities.Remove(opp);
            await _context.SaveChangesAsync();

            // Assert
            var opps = await _context.Opportunities.ToListAsync();
            Assert.That(opps.Count, Is.EqualTo(0));
        }

    }
}
