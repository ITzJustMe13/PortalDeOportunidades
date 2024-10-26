using BackEnd.Controllers.Data;
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
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
        }



    }
}
