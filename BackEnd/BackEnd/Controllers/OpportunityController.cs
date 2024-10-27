using BackEnd.Controllers.Data;
using BackEnd.Models.BackEndModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpportunityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OpportunityController(ApplicationDbContext opportunityContext) => this._context = opportunityContext;

        //GET Opportunity/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OpportunityModel>>> GetAllOpportunities()
        {

        }

        //GET Opportunity/1
        [HttpGet("{id}")]
        public async Task<ActionResult<OpportunityModel>> GetOpportunityById(int id)
        {

        }
    }
}
