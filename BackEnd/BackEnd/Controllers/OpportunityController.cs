using BackEnd.Controllers.Data;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<IEnumerable<Opportunity>>> GetAllOpportunities()
        {
            var opportunities = await _context.Opportunities.ToListAsync();
            if(opportunities == null || !opportunities.Any())
            {
                return NotFound();
            }
            var opportunityDtos = opportunities.Select(OpportunityMapper.MapToDto).ToList();
            return Ok(opportunityDtos);
        }

        //GET Opportunity/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Opportunity>> GetOpportunityById(int id)
        {
            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity == null )
            {
                return NotFound();
            }
            var opportunityDto = OpportunityMapper.MapToDto(opportunity);
            return Ok(opportunityDto);

        }

        //GET Opportunity/User/1
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Opportunity>>> GetAllOpportunitiesByUserId(int userId)
        {
            var opportunities = await _context.Opportunities
                .Where(e => e.User.UserId == userId)
                .ToListAsync();
            if (opportunities == null || !opportunities.Any())
            {
                return NotFound();
            }
            var opportunityDtos = opportunities.Select(OpportunityMapper.MapToDto).ToList();
            return Ok(opportunityDtos);
        }
    }
}
