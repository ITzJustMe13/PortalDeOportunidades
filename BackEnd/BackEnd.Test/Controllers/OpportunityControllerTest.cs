﻿using BackEnd.Controllers.Data;
using BackEnd.Controllers;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models.BackEndModels;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using DotNetEnv;
using BackEnd.Interfaces;
using Moq;
using BackEnd.Services;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Configuration;

namespace BackEnd.Test
{
    [TestFixture]
    public class OpportunityControllerTest
    {
        private OpportunityController _controller;
        private IOpportunityService _opportunityService;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {

            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            // Criação do banco de dados em memória para os testes
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            // Criação do serviço (simulado)
            _opportunityService = new OpportunityService(_context);

            // Instanciação do controlador com a dependência de IConfiguration
            _controller = new OpportunityController(_opportunityService, configuration);

        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllOpportunities_ReturnsOkEnumeratorOfOpportunityDTOs_IfThereAreOpportunities()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "um sitio2", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };


            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetAllOpportunities();

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>(), "Expected OkObjectResult if there are opportunities");

            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "OkObjectResult should not be null");

            var returnedList = okResult.Value as IEnumerable<Opportunity>;
            Assert.That(returnedList!.Count(), Is.EqualTo(2), "Returned enumerable count should be the same as the number of saved opportunities");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllOpportunities_NotFoundObjectResult_IfThereAreNoOpportunities()
        {
            // Arrange

            // Act
            var response = await _controller.GetAllOpportunities();

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult if there are no opportunities");

            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null, "NotFoundObjectResult should not be null");
            Assert.That(notFoundResult?.Value, Is.EqualTo("No Opportunities were found."), "Error message should match the expected not found message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllOpportunities_NotFoundObjectResult_DBContextMissing()
        {
            // Arrange
            var opportunityService = new OpportunityService(null);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            var controller = new OpportunityController(opportunityService, configuration);

            // Act
            var response = await controller.GetAllOpportunities();

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllImpulsedOpportunities_ReturnsOkEnumeratortOfOpportunityDTOs_IfThereAreImpulsedOpportunities()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "um sitio2", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };
            var opportunity3 = new OpportunityModel { OpportunityId = 3, Price = 100, Address = "um sitio3", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);
            _context.Opportunities.Add(opportunity3);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetAllImpulsedOpportunities();

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>(), "Expected OkObjectResult if there are impulsed opportunities");

            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "OkObjectResult should not be null");

            var returnedList = okResult.Value as IEnumerable<Opportunity>;
            Assert.That(returnedList!.Count(), Is.EqualTo(2), "Returned enumerable count should be the same as the number of saved impulsed opportunities");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllImpulsedOpportunities_NotFoundObjectResults_IfThereAreNoImpulsedOpportunities()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "um sitio2", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };


            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetAllImpulsedOpportunities();

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult if there are no impulsed opportunities");

            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null, "NotFoundObjectResult should not be null");
            Assert.That(notFoundResult?.Value, Is.EqualTo("No impulsed opportunities were found."), "Error message should match the expected not found message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllImpulsedOpportunities_NotFoundObjectResult_DBContextMissing()
        {
            // Arrange
            var opportunityService = new OpportunityService(null);

            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            var controller = new OpportunityController(opportunityService, configuration);

            // Act
            var response = await controller.GetAllImpulsedOpportunities();

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetOpportunityById_ReturnsOpportunityDto_ForValidId()
        {
            // Arrange
            var opportunityId = 1;
            var opportunityModel = new OpportunityModel { OpportunityId = opportunityId, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetEntityById(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>(), "Expected OkObjectResult for valid opportunity ID");

            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "OkObjectResult should not be null");

            var returnedOpportunity = okResult.Value as Opportunity;
            Assert.That(returnedOpportunity, Is.TypeOf<Opportunity>(), "Expected Opportunity object for valid opportunityId");
            Assert.That(returnedOpportunity.opportunityId, Is.EqualTo(opportunityId), "Expected returned opportunity id to be the same as the given in the request");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetOpportunityById_ReturnsBadRequestObjectResult_ForInvalidId()
        {
            // Arrange
            var opportunityId = 0;
            var opportunityModel = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetEntityById(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult for invalid opportunity ID");

            var okResult = response as BadRequestObjectResult;
            Assert.That(okResult, Is.Not.Null, "BadRequestObjectResult should not be null");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo($"Given opportunityId is invalid, it should be greater than 0."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetOpportunityById_ReturnsNotFoundObjectResult_ForNonExistentId()
        {
            // Arrange
            var opportunityId = 1;
            var opportunityModel = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetEntityById(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult for nonexistent opportunity ID");

            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null, "NotFoundObjectResult should not be null");
            Assert.That(notFoundResult?.Value, Is.EqualTo($"Opportunity with id {opportunityId} not found."), "Error message should match the expected not found message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetOpportunityById_ReturnsNotFoundObjectResult_DBContextMissing()
        {
            // Arrange
            var opportunityService = new OpportunityService(null);

            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            var controller = new OpportunityController(opportunityService, configuration);

            var opportunityId = 1;
            var opportunityModel = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await controller.GetEntityById(opportunityId);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllOpportunitiesByUserId_ReturnsOkEnumeratortOfOpportunityDTOs_IfThereAreOpportunitiesCreatedByTheGivenUserId()
        {
            // Arrange

            var userId = 1;

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = userId, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "um sitio2", Category = Enums.Category.AGRICULTURA, UserID = userId, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };
            var opportunity3 = new OpportunityModel { OpportunityId = 3, Price = 100, Address = "um sitio3", Category = Enums.Category.AGRICULTURA, UserID = 2, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);
            _context.Opportunities.Add(opportunity3);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetAllOpportunitiesByUserId(userId);

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>(), "Expected OkObjectResult if there are opportunities created by given userId");

            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "OkObjectResult should not be null");

            var returnedList = okResult.Value as IEnumerable<Opportunity>;
            Assert.That(returnedList!.Count(), Is.EqualTo(2), "Returned enumerable count should be the same as the number of saved opportunities created by given userId");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllOpportunitiesByUserId_returnsBadRequestObjectResults_ForInvalidUserId()
        {
            // Arrange
            var opportunityId = 0;
            var opportunityModel = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetAllOpportunitiesByUserId(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult for invalid opportunity ID");

            var okResult = response as BadRequestObjectResult;
            Assert.That(okResult, Is.Not.Null, "BadRequestObjectResult should not be null");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo($"Given userId is invalid, it should be greater than 0."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllOpportunitiesByUserId_returnsNotFoundObjectResults_IfThereAreNoOpportunitiesCreatedByTheGivenUserId()
        {
            // Arrange
            var userId = 2;

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "um sitio2", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.GetAllOpportunitiesByUserId(userId);

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult if there are no opportunities created by given user");

            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null, "NotFoundObjectResult should not be null");
            Assert.That(notFoundResult?.Value, Is.EqualTo($"Opportunities with userId {userId} not found."), "Error message should match the expected not found message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task GetAllOpportunitiesByUserId_returnsNotFoundObjectResults_DBContextMissing()
        {
            // Arrange
            var opportunityService = new OpportunityService(null);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            var controller = new OpportunityController(opportunityService, configuration);

            var userId = 2;

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 100, Address = "um sitio2", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);

            await _context.SaveChangesAsync();

            // Act
            var response = await controller.GetAllOpportunitiesByUserId(userId);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task SearchOpportunities_ReturnsOkEnumeratortOfOpportunityDTOs_IfThereAreOpportunitiesWithGivenParameters()
        {
            // Arrange
            var keyword = "um sitio";
            var vacancies = 1;
            var minPrice = 1;
            var maxPrice = 200;
            var category = Enums.Category.ARTES_CIENCIAS;
            var location = Enums.Location.ACORES;

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = category, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = vacancies, IsActive = true, Location = location, Score = 0, IsImpulsed = true };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 400, Address = "um sitio2", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };
            var opportunity3 = new OpportunityModel { OpportunityId = 3, Price = 400, Address = "um sitio3", Category = Enums.Category.AGRICULTURA, UserID = 2, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);
            _context.Opportunities.Add(opportunity3);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.SearchOpportunities(keyword, vacancies, minPrice, maxPrice, category, location);

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>(), "Expected OkObjectResult if there are opportunities within given parameters");

            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "OkObjectResult should not be null");

            var returnedList = okResult.Value as IEnumerable<Opportunity>;
            Assert.That(returnedList!.Count(), Is.EqualTo(1), "Returned enumerable count should be the same as the number of saved opportunities within given parameters");
        }

        [Test]
        [Category("UnitTest")]
        public async Task SearchOpportunities_ReturnsOkEnumeratortOfOpportunityDTOs_ForOnlyAFewParameters()
        {
            // Arrange
            var keyword = "um sitio";
            var vacancies = 1;
            var minPrice = 1;

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = vacancies, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 400, Address = "um sitio2", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };
            var opportunity3 = new OpportunityModel { OpportunityId = 3, Price = 400, Address = "um sitio3", Category = Enums.Category.AGRICULTURA, UserID = 2, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);
            _context.Opportunities.Add(opportunity3);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.SearchOpportunities(keyword, vacancies, minPrice, null, null, null);

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>(), "Expected OkObjectResult if there are opportunities within given parameters");

            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "OkObjectResult should not be null");

            var returnedList = okResult.Value as IEnumerable<Opportunity>;
            Assert.That(returnedList!.Count(), Is.EqualTo(1), "Returned enumerable count should be the same as the number of saved opportunities within given parameters");
        }

        [Test]
        [Category("UnitTest")]
        public async Task SearchOpportunities_ReturnsOkEnumeratortOfOpportunityDTOs_ForNoParameters()
        {
            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };
            var opportunity2 = new OpportunityModel { OpportunityId = 2, Price = 400, Address = "um sitio2", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };
            var opportunity3 = new OpportunityModel { OpportunityId = 3, Price = 400, Address = "um sitio3", Category = Enums.Category.AGRICULTURA, UserID = 2, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunity);
            _context.Opportunities.Add(opportunity2);
            _context.Opportunities.Add(opportunity3);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.SearchOpportunities(null, null, null, null, null, null);

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>(), "Expected OkObjectResult if there are opportunities");

            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "OkObjectResult should not be null");

            var returnedList = okResult.Value as IEnumerable<Opportunity>;
            Assert.That(returnedList!.Count(), Is.EqualTo(3), "Returned enumerable count should be the same as the number of saved opportunities");
        }

        [Test]
        [Category("UnitTest")]
        public async Task SearchOpportunities_ReturnsBadRequestObjectResult_ForInvalidVacancies()
        {
            var vacancies = -1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = vacancies, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.SearchOpportunities(null, vacancies, null, null, null, null);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected OkObjectResult if there are opportunities");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid search parameters."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task SearchOpportunities_ReturnsBadRequestObjectResult_ForInvalidMinPrice()
        {
            var minPrice = -1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.SearchOpportunities(null, null, minPrice, null, null, null);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected OkObjectResult if there are opportunities");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid search parameters."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task SearchOpportunities_ReturnsBadRequestObjectResult_ForInvalidMaxPrice()
        {
            var maxPrice = -1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.SearchOpportunities(null, null, null, maxPrice, null, null);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected OkObjectResult if there are opportunities");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid search parameters."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task SearchOpportunities_ReturnsBadRequestObjectResult_ForMinPriceGreaterThanMaxPrice()
        {
            var minPrice = 2;
            var maxPrice = 1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.SearchOpportunities(null, null, minPrice, maxPrice, null, null);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected OkObjectResult if there are opportunities");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid search parameters."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task SearchOpportunities_ReturnsBadRequestObjectResult_ForInvalidCategory()
        {

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.SearchOpportunities(null, null, null, null, (Enums.Category?)-1, null);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected OkObjectResult if there are opportunities");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid search parameters."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task SearchOpportunities_ReturnsBadRequestObjectResult_ForInvalidLocation()
        {

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.SearchOpportunities(null, null, null, null, null, (Enums.Location?)-1);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected OkObjectResult if there are opportunities");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid search parameters."), "Error message should match the expected bad request message");
        }


        [Test]
        [Category("UnitTest")]
        public async Task SearchOpportunities_ReturnsNotFoundObjectResult_DBContextMissing()
        {

            // Arrange
            var opportunityService = new OpportunityService(null);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            var controller = new OpportunityController(opportunityService, configuration);


            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await controller.SearchOpportunities(null, null, null, null, null, null);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsCreatedAtActionOpportunityDTO_ForValidOpportunity()
        {
            // Arrange

            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "name",
                price = 12,
                vacancies = 10,
                isActive = true,
                category = 0,
                description = "string",
                location = 0,
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now,
                isImpulsed = true,
                OpportunityImgs = []
            };


            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<CreatedAtActionResult>(), "Expected CreatedAtActionResult if opportunity is valid");

            var createdAtActionResult = response as CreatedAtActionResult;
            Assert.That(createdAtActionResult, Is.Not.Null, "CreatedAtActionResult should not be null");

            var returnedOpportunity = createdAtActionResult.Value as Opportunity;
            Assert.That(returnedOpportunity, Is.Not.Null, "Returned opportunity should not be null");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsBadRequestObjectResult_ForNonExistentUserId()
        {
            // Arrange
            var opportunity = new Opportunity
            {
                name = "name",
                price = 12,
                vacancies = 10,
                isActive = true,
                category = 0,
                description = "string",
                location = 0,
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now,
                isImpulsed = true,
                OpportunityImgs = []
            };


            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if user id is nonexistent");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid User ID. User does not exist."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsBadRequestObjectResult_ForEmptyName()
        {
            // Arrange
            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "",
                price = 12,
                vacancies = 10,
                isActive = true,
                category = 0,
                description = "string",
                location = 0,
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now,
                isImpulsed = true,
                OpportunityImgs = []
            };


            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if name is empty");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Validation failed."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsBadRequestObjectResult_ForLargerThanAcceptedName()
        {
            // Arrange
            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "asdasdasdasdasdasdasdasdasdasdasdasdddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
                price = 12,
                vacancies = 10,
                isActive = true,
                category = 0,
                description = "string",
                location = 0,
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now,
                isImpulsed = true,
                OpportunityImgs = []
            };


            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if name is larger than accepted");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Validation failed."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsBadRequestObjectResult_ForEmptyDescription()
        {
            // Arrange
            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "name",
                price = 12,
                vacancies = 10,
                isActive = true,
                category = 0,
                description = "",
                location = 0,
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now,
                isImpulsed = true,
                OpportunityImgs = []
            };

            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if description is empty");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Validation failed."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsBadRequestObjectResult_ForLargerThanAcceptedDescription()
        {
            // Arrange
            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "name",
                price = 12,
                vacancies = 10,
                isActive = true,
                category = 0,
                description = "assssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                location = 0,
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now,
                isImpulsed = true,
                OpportunityImgs = []
            };


            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if description is larger than accepted");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Validation failed."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsBadRequestObjectResult_ForInvalidPrice()
        {
            // Arrange
            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "name",
                price = 0,
                vacancies = 10,
                isActive = true,
                category = 0,
                description = "description",
                location = 0,
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now,
                isImpulsed = true,
                OpportunityImgs = []
            };

            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if description is empty");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Validation failed."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsBadRequestObjectResult_ForInvalidVacancies()
        {
            // Arrange
            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "name",
                price = 10,
                vacancies = 0,
                isActive = true,
                category = 0,
                description = "description",
                location = 0,
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now,
                isImpulsed = true,
                OpportunityImgs = []
            };

            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if description is empty");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Validation failed."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsBadRequestObjectResult_ForInvalidCategory()
        {
            // Arrange
            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "name",
                price = 10,
                vacancies = 10,
                isActive = true,
                category = (Enums.Category)(-1),
                description = "description",
                location = 0,
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now,
                isImpulsed = true,
                OpportunityImgs = []
            };

            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if description is empty");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Validation failed."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsBadRequestObjectResult_ForInvalidLocation()
        {
            // Arrange
            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "name",
                price = 10,
                vacancies = 10,
                isActive = true,
                category = 0,
                description = "description",
                location = (Enums.Location)(-1),
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now,
                isImpulsed = true,
                OpportunityImgs = []
            };

            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if description is empty");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Validation failed."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsBadRequestObjectResult_ForInvalidAddress()
        {
            // Arrange
            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "name",
                price = 10,
                vacancies = 10,
                isActive = true,
                category = 0,
                description = "description",
                location = 0,
                address = "assssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaasssssssssss",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now,
                isImpulsed = true,
                OpportunityImgs = []
            };

            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if description is empty");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Validation failed."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsBadRequestObjectResult_ForInvalidDate()
        {
            // Arrange
            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "name",
                price = 10,
                vacancies = 10,
                isActive = true,
                category = 0,
                description = "description",
                location = 0,
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now.AddDays(-1),
                isImpulsed = true,
                OpportunityImgs = []
            };

            // Act
            var response = await _controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if description is empty");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Validation failed."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task CreateOpportunity_ReturnsNotFoundObjectResult_DBContextMissing()
        {
            // Arrange
            var opportunityService = new OpportunityService(null);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            var controller = new OpportunityController(opportunityService, configuration);

            var user = new UserModel
            {
                FirstName = "Joel",
                LastName = "Sousa",
                Email = "joel@gmail.com",
                CellPhoneNum = 123456789,
                BirthDate = DateTime.Parse("2004 -11-01T22:17:41.558"),
                Gender = 0,
                Image = [22, 22]
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var opportunity = new Opportunity
            {
                name = "name",
                price = 10,
                vacancies = 10,
                isActive = true,
                category = 0,
                description = "description",
                location = 0,
                address = "string",
                userId = 1,
                reviewScore = 0,
                date = DateTime.Now.AddDays(12),
                isImpulsed = true,
                OpportunityImgs = []
            };

            // Act
            var response = await controller.CreateEntity(opportunity);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteOpportunityById_ReturnsNoContentResult_ForValidOpportunityId()
        {
            var opportunityId = 1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = opportunityId, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.DeleteEntity(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<NoContentResult>(), "Expected NoContentResult if the opportunity id is valid");
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteOpportunityById_ReturnsNotFoundObjectResult_ForNonExistentOpportunityId()
        {
            var opportunityId = 1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 2, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.DeleteEntity(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult if the id is non existent");

            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null, "NotFoundObjectResult should not be null");
            Assert.That(notFoundResult?.Value, Is.EqualTo($"Opportunity with id {opportunityId} not found."), "Error message should match the expected not found message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteOpportunityById_ReturnsBadRequestObjectResult_ForInvalidOpportunityId()
        {
            var opportunityId = 0;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.DeleteEntity(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the id is invalid");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Given opportunityId is invalid, it should be greater than 0."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteOpportunityById_ReturnsBadRequestObjectResult_ForOpportunityWithActiveReservations()
        {
            var opportunityId = 1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = opportunityId, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };
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
            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.DeleteEntity(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the opportunity has active reservations");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("This Opportunity still has active reservations attached."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeleteOpportunityById_ReturnsNotFoundObjectResult_DBContextMissing()
        {
            // Arrange
            var opportunityService = new OpportunityService(null);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            var controller = new OpportunityController(opportunityService, configuration);

            var opportunityId = 1;

            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await controller.DeleteEntity(opportunityId);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task ActivateOpportunityById_ReturnsNoContentResult_ForValidOpportunityId()
        {
            var opportunityId = 1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = opportunityId, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = false, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.ActivateOpportunityById(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<NoContentResult>(), "Expected NoContentResult if the opportunity id is valid");
        }

        [Test]
        [Category("UnitTest")]
        public async Task ActivateOpportunityById_ReturnsNotFoundObjectResult_ForNonExistentOpportunityId()
        {
            var opportunityId = 1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 2, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = false, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.ActivateOpportunityById(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult if the id is non existent");

            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null, "NotFoundObjectResult should not be null");
            Assert.That(notFoundResult?.Value, Is.EqualTo($"Opportunity with id {opportunityId} not found."), "Error message should match the expected not found message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task ActivateOpportunityById_ReturnsBadRequestObjectResult_ForInvalidOpportunityId()
        {
            var opportunityId = 0;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = false, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.ActivateOpportunityById(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the id is invalid");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Given opportunityId is invalid, it should be greater than 0."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task ActivateOpportunityById_ReturnsBadRequestObjectResult_ForAlreadyActivatedOpportunity()
        {
            var opportunityId = 1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.ActivateOpportunityById(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the opportunity is already activated");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Opportunity is already active."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task ActivateOpportunityById_ReturnsNotFoundObjectResult_DBContextMissing()
        {
            // Arrange
            var opportunityService = new OpportunityService(null);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            var controller = new OpportunityController(opportunityService, configuration);

            var opportunityId = 1;
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await controller.ActivateOpportunityById(opportunityId);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeactivateOpportunityById_ReturnsNoContentResult_ForValidOpportunityId()
        {
            var opportunityId = 1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = opportunityId, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.DeactivateOpportunityById(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<NoContentResult>(), "Expected NoContentResult if the opportunity id is valid");
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeactivateOpportunityById_ReturnsNotFoundObjectResult_ForNonExistentOpportunityId()
        {
            var opportunityId = 1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 2, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.DeactivateOpportunityById(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult if the id is non existent");

            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null, "NotFoundObjectResult should not be null");
            Assert.That(notFoundResult?.Value, Is.EqualTo($"Opportunity with id {opportunityId} not found."), "Error message should match the expected not found message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeactivateOpportunityById_ReturnsBadRequestObjectResult_ForInvalidOpportunityId()
        {
            var opportunityId = 0;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.DeactivateOpportunityById(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the id is invalid");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Given opportunityId is invalid, it should be greater than 0."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeactivateOpportunityById_ReturnsBadRequestObjectResult_ForAlreadyDeactivatedOpportunity()
        {
            var opportunityId = 1;

            // Arrange
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = false, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await _controller.DeactivateOpportunityById(opportunityId);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the opportunity is already deactivated");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Opportunity is already inactive."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task DeactivateOpportunityById_ReturnsNotFoundObjectResult_DBContextMissing()
        {
            // Arrange
            var opportunityService = new OpportunityService(null);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            var controller = new OpportunityController(opportunityService, configuration);

            var opportunityId = 1;
            var opportunity = new OpportunityModel { OpportunityId = 1, Price = 1, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "um sitio", Date = DateTime.Now.AddDays(30), Vacancies = 1, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = true };

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            // Act
            var response = await controller.DeactivateOpportunityById(opportunityId);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditOpportunityById_ReturnsOkOpportunityDto_ForValidValues()
        {
            // Arrange
            var opportunityId = 1;

            // Create an existing OpportunityModel
            var opportunityModel = new OpportunityModel
            {
                OpportunityId = opportunityId,
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

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            // Example image data (byte array)
            var image = new byte[] { 0xFF, 0xFF, 0x00, 0x00 };

            var oppImg = new OpportunityImg { image = image, opportunityId = opportunityId };
            var oppImgList = new List<OpportunityImg> { oppImg , oppImg};

            var opportunityDto = new Opportunity
            {
                name = "oportunidade 1",
                description = "description1",
                userId = 1,
                price = 255,
                vacancies = 11,
                category = Enums.Category.DESPORTOS_ATIVIDADES_AO_AR_LIVRE,
                location = Enums.Location.SETUBAL,
                address = "Rua da Oportunidade 1",
                date = DateTime.Now.AddDays(45),
                isImpulsed = false,
                OpportunityImgs = oppImgList
            };

            // Act
            var response = await _controller.UpdateEntity(opportunityId, opportunityDto);

            // Assert
            Assert.That(response, Is.TypeOf<OkObjectResult>(), "Expected OkObjectResult for valid opportunity ID and valid values");

            var okResult = response as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "OkObjectResult should not be null");

            var returnedOpportunity = okResult.Value as Opportunity;
            Assert.That(returnedOpportunity, Is.Not.Null, "Expected Opportunity object for valid opportunityId");
            Assert.That(returnedOpportunity.opportunityId, Is.EqualTo(opportunityId), "Expected returned opportunity ID to match the request ID");
            Assert.That(returnedOpportunity.name, Is.EqualTo(opportunityDto.name), "Expected returned name to match the updated value");
            Assert.That(returnedOpportunity.description, Is.EqualTo(opportunityDto.description), "Expected returned description to match the updated value");
            Assert.That(returnedOpportunity.price, Is.EqualTo(opportunityDto.price), "Expected returned price to match the updated value");
            Assert.That(returnedOpportunity.vacancies, Is.EqualTo(opportunityDto.vacancies), "Expected returned vacancies to match the updated value");
            Assert.That(returnedOpportunity.category, Is.EqualTo(opportunityDto.category), "Expected returned category to match the updated value");
            Assert.That(returnedOpportunity.location, Is.EqualTo(opportunityDto.location), "Expected returned location to match the updated value");
            Assert.That(returnedOpportunity.address, Is.EqualTo(opportunityDto.address), "Expected returned address to match the updated value");
            Assert.That(returnedOpportunity.date, Is.EqualTo(opportunityDto.date), "Expected returned date to match the updated value");

            // Validate returned images
            Assert.That(returnedOpportunity.OpportunityImgs, Is.Not.Null, "Expected OpportunityImgs to not be null");
            Assert.That(returnedOpportunity.OpportunityImgs.Count, Is.EqualTo(opportunityDto.OpportunityImgs.Count), "Expected OpportunityImgs count to match the request");

            var expectedImages = oppImgList.Select(img => img.image).ToList(); // Extract expected images (byte[])

            Assert.That(returnedOpportunity.OpportunityImgs.Count, Is.EqualTo(expectedImages.Count),
                "Expected the number of returned images to match the given images");

            int index = 0;
            foreach (var actualImageObj in returnedOpportunity.OpportunityImgs)
            {
                var actualImage = actualImageObj.image; // Access the byte[] from the current image
                var expectedImage = expectedImages[index]; // Get the corresponding expected image
                Assert.That(actualImage, Is.EqualTo(expectedImage),
                    $"Expected image at index {index} to match the given image");
                index++;
            }
        }


        [Test]
        [Category("UnitTest")]
        public async Task EditOpportunityById_ReturnsBadRequestObjectResult_ForInvalidId()
        {
            // Arrange
            var opportunityId = 0;
            var opportunityModel = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            var image = new byte[] { 0xFF, 0xFF, 0x00, 0x00 };

            var oppImg = new OpportunityImg { image = image, opportunityId = opportunityId };
            var oppImgList = new List<OpportunityImg> { oppImg, oppImg };

            var opportunityDto = new Opportunity
            {
                name = "oportunidade 1",
                description = "description1",
                userId = 1,
                price = 255,
                vacancies = 11,
                category = Enums.Category.DESPORTOS_ATIVIDADES_AO_AR_LIVRE,
                location = Enums.Location.SETUBAL,
                address = "Rua da Oportunidade 1",
                date = DateTime.Now.AddDays(45),
                isImpulsed = false,
                OpportunityImgs = oppImgList
            };

            // Act
            var response = await _controller.UpdateEntity(opportunityId, opportunityDto);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the id is invalid");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Given opportunityId is invalid, it should be greater than 0."), "Error message should match the expected bad request message");
        }


        [Test]
        [Category("UnitTest")]
        public async Task EditOpportunityById_ReturnsBadRequestObjectResult_ForNonExistentId()
        {
            // Arrange
            var opportunityId = 2;
            var opportunityModel = new OpportunityModel { OpportunityId = 1, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            var image = new byte[] { 0xFF, 0xFF, 0x00, 0x00 };

            var oppImg = new OpportunityImg { image = image, opportunityId = opportunityId };
            var oppImgList = new List<OpportunityImg> { oppImg, oppImg };

            var opportunityDto = new Opportunity
            {
                name = "oportunidade 1",
                description = "description1",
                userId = 1,
                price = 255,
                vacancies = 11,
                category = Enums.Category.DESPORTOS_ATIVIDADES_AO_AR_LIVRE,
                location = Enums.Location.SETUBAL,
                address = "Rua da Oportunidade 1",
                date = DateTime.Now.AddDays(45),
                isImpulsed = false,
                OpportunityImgs = oppImgList
            };

            // Act
            var response = await _controller.UpdateEntity(opportunityId, opportunityDto);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the id is non existent");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo($"Opportunity with id {opportunityId} not found."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditOpportunityById_ReturnsBadRequestObjectResult_ForValidIdAndInvalidPrice()
        {
            // Arrange
            var opportunityId = 1;
            var opportunityModel = new OpportunityModel { OpportunityId = opportunityId, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            var image = new byte[] { 0xFF, 0xFF, 0x00, 0x00 };

            var oppImg = new OpportunityImg { image = image, opportunityId = opportunityId };
            var oppImgList = new List<OpportunityImg> { oppImg, oppImg };

            var opportunityDto = new Opportunity
            {
                name = "oportunidade 1",
                description = "description1",
                userId = 1,
                price = 0,
                vacancies = 11,
                category = Enums.Category.DESPORTOS_ATIVIDADES_AO_AR_LIVRE,
                location = Enums.Location.SETUBAL,
                address = "Rua da Oportunidade 1",
                date = DateTime.Now.AddDays(45),
                isImpulsed = false,
                OpportunityImgs = oppImgList
            };

            // Act
            var response = await _controller.UpdateEntity(opportunityId, opportunityDto);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the id is valid and the price is invalid");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Price should be at least 0.01."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditOpportunityById_ReturnsBadRequestObjectResult_ForValidIdAndInvalidVacancies()
        {
            // Arrange
            var opportunityId = 1;
            var opportunityModel = new OpportunityModel { OpportunityId = opportunityId, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            var image = new byte[] { 0xFF, 0xFF, 0x00, 0x00 };

            var oppImg = new OpportunityImg { image = image, opportunityId = opportunityId };
            var oppImgList = new List<OpportunityImg> { oppImg, oppImg };

            var opportunityDto = new Opportunity
            {
                name = "oportunidade 1",
                description = "description1",
                userId = 1,
                price = 255,
                vacancies = 0,
                category = Enums.Category.DESPORTOS_ATIVIDADES_AO_AR_LIVRE,
                location = Enums.Location.SETUBAL,
                address = "Rua da Oportunidade 1",
                date = DateTime.Now.AddDays(45),
                isImpulsed = false,
                OpportunityImgs = oppImgList
            };

            // Act
            var response = await _controller.UpdateEntity(opportunityId, opportunityDto);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the id is valid and the vacancies is invalid");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Vacancies should be at least one."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditOpportunityById_ReturnsBadRequestObjectResult_ForValidIdAndInvalidCategory()
        {
            // Arrange
            var opportunityId = 1;
            var opportunityModel = new OpportunityModel { OpportunityId = opportunityId, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            var image = new byte[] { 0xFF, 0xFF, 0x00, 0x00 };

            var oppImg = new OpportunityImg { image = image, opportunityId = opportunityId };
            var oppImgList = new List<OpportunityImg> { oppImg, oppImg };

            var opportunityDto = new Opportunity
            {
                name = "oportunidade 1",
                description = "description1",
                userId = 1,
                price = 255,
                vacancies = 11,
                category = (Enums.Category)9999,
                location = Enums.Location.SETUBAL,
                address = "Rua da Oportunidade 1",
                date = DateTime.Now.AddDays(45),
                isImpulsed = false,
                OpportunityImgs = oppImgList
            };


            // Act
            var response = await _controller.UpdateEntity(opportunityId, opportunityDto);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the id is valid and the Category is invalid");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Category is not valid."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditOpportunityById_ReturnsBadRequestObjectResult_ForValidIdAndInvalidLocation()
        {
            // Arrange
            var opportunityId = 1;
            var opportunityModel = new OpportunityModel { OpportunityId = opportunityId, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            var image = new byte[] { 0xFF, 0xFF, 0x00, 0x00 };

            var oppImg = new OpportunityImg { image = image, opportunityId = opportunityId };
            var oppImgList = new List<OpportunityImg> { oppImg, oppImg };

            var opportunityDto = new Opportunity
            {
                name = "oportunidade 1",
                description = "description1",
                userId = 1,
                price = 255,
                vacancies = 11,
                category = Enums.Category.DESPORTOS_ATIVIDADES_AO_AR_LIVRE,
                location = (Enums.Location)9999,
                address = "Rua da Oportunidade 1",
                date = DateTime.Now.AddDays(45),
                isImpulsed = false,
                OpportunityImgs = oppImgList
            };

            // Act
            var response = await _controller.UpdateEntity(opportunityId, opportunityDto);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the id is valid and the Location is invalid");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Location is not valid."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditOpportunityById_ReturnsBadRequestObjectResult_ForValidIdAndInvalidAddress()
        {
            // Arrange
            var opportunityId = 1;
            var opportunityModel = new OpportunityModel { OpportunityId = opportunityId, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            var image = new byte[] { 0xFF, 0xFF, 0x00, 0x00 };

            var oppImg = new OpportunityImg { image = image, opportunityId = opportunityId };
            var oppImgList = new List<OpportunityImg> { oppImg, oppImg };

            var opportunityDto = new Opportunity
            {
                name = "oportunidade 1",
                description = "description1",
                userId = 1,
                price = 255,
                vacancies = 11,
                category = Enums.Category.DESPORTOS_ATIVIDADES_AO_AR_LIVRE,
                location = Enums.Location.SETUBAL,
                address = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                date = DateTime.Now.AddDays(45),
                isImpulsed = false,
                OpportunityImgs = oppImgList
            };
           
            // Act
            var response = await _controller.UpdateEntity(opportunityId, opportunityDto);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the id is valid and the Address is invalid");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Address should be 200 characters or less."), "Error message should match the expected bad request message");
        }

        [Test]
        [Category("UnitTest")]
        public async Task EditOpportunityById_ReturnsBadRequestObjectResult_ForValidIdAndInvalidDate()
        {
            // Arrange
            var opportunityId = 1;
            var opportunityModel = new OpportunityModel { OpportunityId = opportunityId, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            var image = new byte[] { 0xFF, 0xFF, 0x00, 0x00 };

            var oppImg = new OpportunityImg { image = image, opportunityId = opportunityId };
            var oppImgList = new List<OpportunityImg> { oppImg, oppImg };

            var opportunityDto = new Opportunity
            {
                name = "oportunidade 1",
                description = "description1",
                userId = 1,
                price = 255,
                vacancies = 11,
                category = Enums.Category.DESPORTOS_ATIVIDADES_AO_AR_LIVRE,
                location = Enums.Location.SETUBAL,
                address = "Rua da Oportunidade 1",
                date = DateTime.Now.AddDays(-45),
                isImpulsed = false,
                OpportunityImgs = oppImgList
            };

            // Act
            var response = await _controller.UpdateEntity(opportunityId, opportunityDto);

            // Assert
            Assert.That(response, Is.TypeOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult if the id is valid and the Location is invalid");

            var badRequestResult = response as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult should not be null");
            Assert.That(badRequestResult?.Value, Is.EqualTo("Date must be in the future."), "Error message should match the expected bad request message");
        }


        [Test]
        [Category("UnitTest")]
        public async Task EditOpportunityById_ReturnsNotFoundObjectResult_DBContextMissing()
        {
            // Arrange
            var opportunityService = new OpportunityService(null);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MessageMode", "Development" }  // ou "Production"
            })
            .Build();

            var controller = new OpportunityController(opportunityService, configuration);

            var opportunityId = 1;
            var opportunityModel = new OpportunityModel { OpportunityId = opportunityId, Price = 100, Address = "um sitio", Category = Enums.Category.AGRICULTURA, UserID = 1, Name = "name", Description = "a description", Date = DateTime.Now.AddDays(30), Vacancies = 2, IsActive = true, Location = Enums.Location.LISBOA, Score = 0, IsImpulsed = false };

            _context.Opportunities.Add(opportunityModel);
            await _context.SaveChangesAsync();

            var image = new byte[] { 0xFF, 0xFF, 0x00, 0x00 };

            var oppImg = new OpportunityImg { image = image, opportunityId = opportunityId };
            var oppImgList = new List<OpportunityImg> { oppImg, oppImg };

            var opportunityDto = new Opportunity
            {
                name = "oportunidade 1",
                description = "description1",
                userId = 1,
                price = 255,
                vacancies = 10,
                category = Enums.Category.DESPORTOS_ATIVIDADES_AO_AR_LIVRE,
                location = Enums.Location.SETUBAL,
                address = "Rua da Oportunidade 1",
                date = DateTime.Now.AddDays(45),
                isImpulsed = false,
                OpportunityImgs = oppImgList
            };

            // Act
            var response = await controller.UpdateEntity(opportunityId, opportunityDto);

            // Assert
            Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = response as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("DB context is missing."));
        }
    }
}
