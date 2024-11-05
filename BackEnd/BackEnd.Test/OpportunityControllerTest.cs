using BackEnd.Controllers.Data;
using BackEnd.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.Models.BackEndModels;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;

namespace BackEnd.Test
{
    [TestFixture]
    public class OpportunityControllerTest
    {
        private OpportunityController _controller;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            // Use in-memory database for ApplicationDbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new OpportunityController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllOpportunities_ReturnsOkListOfOpportunityDTOs_IfThereAreOpportunities()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "um sitio2", Category = Enums.Category.AGRICULTURA, userID = 1, Name = "name", Description = "a description", date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };


            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetAllOpportunities();

            // Assert
            Assert.That(response.Result, Is.TypeOf<OkObjectResult>(), "Expected OkObjectResult if there are opportunities");

            var okResult = response.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "OkObjectResult should not be null");

            var returnedList = okResult.Value as IEnumerable<Opportunity>;
            Assert.That(returnedList!.Count(), Is.EqualTo(2));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllOpportunities_ReturnsOkListOfOpportunityDTOs_IfThereAreNoOpportunities()
        {
            // Arrange

            // Act
            var response = await _controller.GetAllOpportunities();

            // Assert
            Assert.That(response.Result, Is.TypeOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult if there are opportunities");

            var notFoundResult = response.Result as OkObjectResult;
            Assert.That(notFoundResult, Is.Null, "NotFoundObjectResult should be null");
        }
    }
}
