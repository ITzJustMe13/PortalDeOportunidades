using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpportunityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OpportunityController(ApplicationDbContext opportunityContext) => this._context = opportunityContext;

        //GET api/Opportunity/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Opportunity>>> GetAllOpportunities()
        {
            var opportunityModels = await _context.Opportunities.ToListAsync();
            if (!opportunityModels.Any())
            {
                return NotFound("No Opportunities were found.");
            }
            var opportunityDtos = opportunityModels.Select(OpportunityMapper.MapToDto).ToList();
            return Ok(opportunityDtos);
        }

        //GET api/Opportunity/Impulsed
        [HttpGet("Impulsed")]
        public async Task<ActionResult<IEnumerable<Opportunity>>> GetAllImpulsedOpportunities()
        {
            var opportunityModels = await _context.Opportunities
                .Where(o => o.IsImpulsed == true)
                .ToListAsync();

            if (!opportunityModels.Any())
            {
                return NotFound("No Opportunities were found.");
            }

            var opportunitiesDto = opportunityModels.Select(OpportunityMapper.MapToDto).ToList();
            return Ok(opportunitiesDto);
        }

        //GET api/Opportunity/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Opportunity>> GetOpportunityById(int id)
        {
            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity == null)
            {
                return NotFound($"Opportunity with id {id} not found.");
            }
            var opportunityDto = OpportunityMapper.MapToDto(opportunity);
            return Ok(opportunityDto);

        }

        //GET api/Opportunity/User/1
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Opportunity>>> GetAllOpportunitiesByUserId(int userId)
        {
            var opportunities = await _context.Opportunities
                .Where(e => e.User.UserId == userId)
                .ToListAsync();
            if (opportunities == null || !opportunities.Any())
            {
                return NotFound($"Opportunity with userId {userId} not found.");
            }
            var opportunityDtos = opportunities.Select(OpportunityMapper.MapToDto).ToList();
            return Ok(opportunityDtos);
        }

        //GET api/Opportunity/Search?keyword=event&vacancies=5&minPrice=10&maxPrice=100&category=conference&location=VilaReal
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Opportunity>>> SearchOpportunities(
            [FromQuery] string? keyword,
            [FromQuery] int? vacancies,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] Category? category,
            [FromQuery] Location? location
            )
        {
            var query = _context.Opportunities.AsQueryable();

            //Keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(o => o.Name.Contains(keyword) || o.Description.Contains(keyword));
            }

            //Vacancies
            if (vacancies.HasValue)
            {
                query = query.Where(o => o.Vacancies >= vacancies.Value);
            }

            //MinPrice
            if (minPrice.HasValue)
            {
                query = query.Where(o => o.Price >= minPrice.Value);
            }

            //MaxPrice
            if (maxPrice.HasValue)
            {
                query = query.Where(o => o.Price <= maxPrice.Value);
            }

            //Category
            if (category.HasValue)
            {
                query = query.Where(o => o.Category == category);
            }

            //Location
            if (location.HasValue)
            {
                query = query.Where(o => o.Location == location);
            }

            var opportunitiesModels = await query.ToListAsync();
            var opportunityDtos = opportunitiesModels.Select(OpportunityMapper.MapToDto).ToList();

            return Ok(opportunityDtos);
        }

        //POST api/Opportunity/
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Opportunity>> CreateOpportunity(Opportunity opportunity)
        {
            if(opportunity == null)
            {
                return BadRequest("Opportunity Data is Required.");
            }
            if(opportunity.userId == null || opportunity.userId < 0)
            {
                return BadRequest("Invalid User Id.");
            }
            var opportunityModel = OpportunityMapper.MapToModel(opportunity);
            await _context.Opportunities.AddAsync(opportunityModel);
            await _context.SaveChangesAsync();

            var createdOpportunityDto = OpportunityMapper.MapToDto(opportunityModel);
            return CreatedAtAction(nameof(GetOpportunityById), new { id = createdOpportunityDto.opportunityId }, createdOpportunityDto);
        }

        //DELETE api/Opportunity/
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Opportunity>> DeleteOpportunityById(int id)
        {
            var opportunityModel = await _context.Opportunities.FindAsync(id);

            if(opportunityModel == null)
            {
                return NotFound($"Opportunity with id {id} not found.");
            }

            bool hasActiveReservations = await _context.Reservations
                .AnyAsync(r => r.opportunityID == id && r.isActive);

            if (hasActiveReservations)
            {
                // Has active reservations, not safe to delete
                return BadRequest("This Opportunity still has active reservations attached.");
            }

            // No active reservations, safe to delete
            _context.Opportunities.Remove(opportunityModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //PUT api/Opportunity/1/activate
        [HttpPut("{id}/activate")]
        [Authorize]
        public async Task<ActionResult<Opportunity>> ActivateOpportunityById(int id)
        {
            var opportunityModel = await _context.Opportunities.FindAsync(id);
            if (opportunityModel == null)
            {
                return NotFound($"Opportunity with id {id} not found.");
            }
            if (!opportunityModel.IsActive) 
            {
                opportunityModel.IsActive = true;
                await _context.SaveChangesAsync();
                return NoContent();

            } else
            {
                return BadRequest("Opportunity is Already Active");
            }

        }

        //PUT api/Opportunity/1/deactivate
        [HttpPut("{id}/deactivate")]
        [Authorize]
        public async Task<ActionResult<Opportunity>>DeactivateOpportunityById(int id)
        {
            var opportunityModel = await _context.Opportunities.FindAsync(id);
            if (opportunityModel == null)
            {
                return NotFound($"Opportunity with id {id} not found.");
            }
            if (opportunityModel.IsActive)
            {
                opportunityModel.IsActive = false;
                await _context.SaveChangesAsync();
                return NoContent();

            } else
            {
                return BadRequest("Opportunity is Already Inactive");
            }

        }

        //PUT api/Opportunity/1/Impulse?days=30
        [HttpPut("{id}/Impulse")]
        [Authorize]
        public async Task<ActionResult<Opportunity>> ActivateImpulseById(int id, [FromQuery]int days)
        {
            var opportunityModel = await _context.Opportunities.FindAsync(id);
            if(opportunityModel == null)
            {
                return NotFound($"Opportunity with id {id} not found.");
            }
            if(opportunityModel.IsImpulsed == true)
            {
                return BadRequest($"Opportunity already Impulsed, ends in: {opportunityModel.Impulse.ExpireDate}");
            } 
            if (days < 1)
            {
                return BadRequest("Impulse days must be a positive number.");
            } 

            DateTime expDate = DateTime.Today.AddDays(days);
            opportunityModel.IsImpulsed = true;
            opportunityModel.Impulse.ExpireDate = expDate;

            await _context.SaveChangesAsync();
            return NoContent();
        

        }

        //PUT api/Opportunity/1/Edit?name=event&description=description&price=10&vacancies=2&category=agricultura&location=VilaReal&address=RuaTeste&date=10/02/2025
        [HttpPut("{id}/Edit")]
        [Authorize]
        public async Task<ActionResult<Opportunity>> EditOpportunityById (
            int id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] decimal? price,
            [FromQuery] int? vacancies,
            [FromQuery] Category? category,
            [FromQuery] Location? location,
            [FromQuery] string? address,
            [FromQuery] DateTime date
            )
        {
            var opportunityModel = await _context.Opportunities.FindAsync(id);
            if (opportunityModel == null)
            {
                return BadRequest($"Opportunity with id {id} not found.");
            }

            //Name
            if (!string.IsNullOrEmpty(name))
            {
                opportunityModel.Name = name;
            }

            //Description
            if (!string.IsNullOrEmpty(description))
            {
                opportunityModel.Description = description;
            }

            //Price
            if (price != null)
            {
                opportunityModel.Price = (decimal)price;
            }

            //Vacancies
            if(vacancies != null)
            {
                opportunityModel.Vacancies = (int)vacancies;
            }

            //Category
            if(category != null)
            {
                opportunityModel.Category = (Category)category;
            }

            //Location
            if(location != null) 
            {
                opportunityModel.Location = (Location)location;
            }

            //Address
            if(!string.IsNullOrEmpty(address))
            { 
                opportunityModel.Address = address;
            }

            //Date
            opportunityModel.date = date;

            await _context.SaveChangesAsync();

            var opportunityDto = OpportunityMapper.MapToDto(opportunityModel);
            return Ok(opportunityDto);

        }

    }
}
