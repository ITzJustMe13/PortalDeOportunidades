using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.Models.BackEndModels;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;

namespace BackEnd.Tests.Models

{
    [TestFixture]
    public class ReservationModelTest
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

        private OpportunityModel CreateSampleOpportunity(string Name, string Description, decimal Price, int Vacancies, bool IsActive, Category Category, Location Location, string Address, float Score, bool IsImpulsed)
        {
            return new OpportunityModel
            {
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

        private ReservationModel CreateSampleReservation(int opportunityID, int userID, DateTime checkInDate, int numOfPeople, bool isActive)
        {
            return new ReservationModel
            {
                opportunityID = opportunityID,
                userID = userID,
                checkInDate = checkInDate,
                numOfPeople = numOfPeople,
                isActive = isActive
                // `reservationDate` é gerado automaticamente pelo banco de dados
            };
        }

        [Test]
        [Category("UnitTest")]
        public async Task AddReservation_ShouldAddReservationToDatabase()
        {
            // Arrange
            var opportunity = CreateSampleOpportunity(
                "Tour Vinícola",
                "Experiência única em uma vinícola local",
                99.99m,
                20,
                IsActive: true,
                Category.AGRICULTURA,
                Location.PORTO,
                "Rua das Vinhas, 123",
                4.5f,
                IsImpulsed: false
            );

            var user = CreateSampleUser(
                "samplehashedpassword", 
                null, 
                "John", 
                "Doe", 
                "johnDoe@gmail.com", 
                911632142, 
                new DateTime(1990, 1, 1), 
                Gender.MASCULINO, "sampleToken", 
                DateTime.Today.AddDays(1)
                );

            // Instância de uma reserva de exemplo usando o ID do utilizador e da oportunidade
            var reservation = CreateSampleReservation(
                opportunityID: opportunity.OpportunityId,  // ID da oportunidade
                userID: user.UserId,                       // ID do utilizador
                checkInDate: DateTime.Today.AddDays(10),   // Data de check-in daqui a 10 dias
                numOfPeople: 2,                            // Número de pessoas
                isActive: true                             // Reserva ativa
            );

            // Act
            await _context.Users.AddAsync(user);
            await _context.Opportunities.AddAsync(opportunity);
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            // Assert
            var reservations = await _context.Reservations.ToListAsync();
            Assert.That(reservations.Count, Is.EqualTo(1));
            Assert.That(reservations.First().checkInDate, Is.EqualTo(DateTime.Today.AddDays(10)));
            Assert.That(reservations.First().numOfPeople, Is.EqualTo(2));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetReservationById_ShouldReturnReservation()
        {
            // Arrange
            var opportunity = CreateSampleOpportunity(
                "Tour Vinícola",
                "Experiência única em uma vinícola local",
                99.99m,
                20,
                IsActive: true,
                Category.AGRICULTURA,
                Location.PORTO,
                "Rua das Vinhas, 123",
                4.5f,
                IsImpulsed: false
            );

            var user = CreateSampleUser(
                "samplehashedpassword",
                null,
                "John",
                "Doe",
                "johnDoe@gmail.com",
                911632142,
                new DateTime(1990, 1, 1),
                Gender.MASCULINO, "sampleToken",
                DateTime.Today.AddDays(1)
                );

            // Instância de uma reserva de exemplo usando o ID do utilizador e da oportunidade
            var reservation = CreateSampleReservation(
                opportunityID: opportunity.OpportunityId,  // ID da oportunidade
                userID: user.UserId,                       // ID do utilizador
                checkInDate: DateTime.Today.AddDays(10),   // Data de check-in daqui a 10 dias
                numOfPeople: 2,                            // Número de pessoas
                isActive: true                             // Reserva ativa
            );

            // Act
            await _context.Users.AddAsync(user);
            await _context.Opportunities.AddAsync(opportunity);
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            // Act
            var retrievedReservation = await _context.Reservations.FindAsync(reservation.reservationID);

            // Assert
            Assert.That(retrievedReservation, Is.Not.Null);
            Assert.That(retrievedReservation.checkInDate, Is.EqualTo(DateTime.Today.AddDays(10)));
        }

        [Test]
        [Category("UnitTest")]
        public async Task UpdateReservation_ShouldModifyReservationDetails()
        {
            // Arrange
            var opportunity = CreateSampleOpportunity(
                "Tour Vinícola",
                "Experiência única em uma vinícola local",
                99.99m,
                20,
                IsActive: true,
                Category.AGRICULTURA,
                Location.PORTO,
                "Rua das Vinhas, 123",
                4.5f,
                IsImpulsed: false
            );

            var user = CreateSampleUser(
                "samplehashedpassword",
                null,
                "John",
                "Doe",
                "johnDoe@gmail.com",
                911632142,
                new DateTime(1990, 1, 1),
                Gender.MASCULINO, "sampleToken",
                DateTime.Today.AddDays(1)
                );

            // Instância de uma reserva de exemplo usando o ID do utilizador e da oportunidade
            var reservation = CreateSampleReservation(
                opportunityID: opportunity.OpportunityId,  // ID da oportunidade
                userID: user.UserId,                       // ID do utilizador
                checkInDate: DateTime.Today.AddDays(10),   // Data de check-in daqui a 10 dias
                numOfPeople: 2,                            // Número de pessoas
                isActive: true                             // Reserva ativa
            );

            // Act
            await _context.Users.AddAsync(user);
            await _context.Opportunities.AddAsync(opportunity);
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            // Act
            var retrievedReservation = await _context.Reservations.FindAsync(reservation.reservationID);
            retrievedReservation.checkInDate = DateTime.Today.AddDays(20);
            retrievedReservation.numOfPeople = 3;
            await _context.SaveChangesAsync();

            // Assert
            var updatedReservation = await _context.Reservations.FindAsync(reservation.reservationID);
            Assert.That(updatedReservation.checkInDate, Is.EqualTo(DateTime.Today.AddDays(20)));
            Assert.That(updatedReservation.numOfPeople, Is.EqualTo(3));
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteReservation_ShouldRemoveReservationFromDatabase()
        {
            // Arrange
            var opportunity = CreateSampleOpportunity(
                "Tour Vinícola",
                "Experiência única em uma vinícola local",
                99.99m,
                20,
                IsActive: true,
                Category.AGRICULTURA,
                Location.PORTO,
                "Rua das Vinhas, 123",
                4.5f,
                IsImpulsed: false
            );

            var user = CreateSampleUser(
                "samplehashedpassword",
                null,
                "John",
                "Doe",
                "johnDoe@gmail.com",
                911632142,
                new DateTime(1990, 1, 1),
                Gender.MASCULINO, "sampleToken",
                DateTime.Today.AddDays(1)
                );

            // Instância de uma reserva de exemplo usando o ID do utilizador e da oportunidade
            var reservation = CreateSampleReservation(
                opportunityID: opportunity.OpportunityId,  // ID da oportunidade
                userID: user.UserId,                       // ID do utilizador
                checkInDate: DateTime.Today.AddDays(10),   // Data de check-in daqui a 10 dias
                numOfPeople: 2,                            // Número de pessoas
                isActive: true                             // Reserva ativa
            );

            // Act
            await _context.Users.AddAsync(user);
            await _context.Opportunities.AddAsync(opportunity);
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            // Act
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            // Assert
            var reservations = await _context.Reservations.ToListAsync();
            Assert.That(reservations.Count, Is.EqualTo(0));
        }
    }
}
