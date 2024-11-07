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
using System.ComponentModel.DataAnnotations;
using System.Security;
using BackEnd.Services;

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
            if (_context == null)
                return NotFound("DB context missing");

            var opportunityModels = await _context.Opportunities
                .Include(o => o.OpportunityImgs) // Include images
                .ToListAsync();

            if (!opportunityModels.Any())
            {
                return NotFound("No Opportunities were found.");
            }
            try
            {
                var opportunityDtos = opportunityModels.Select(OpportunityMapper.MapToDto).ToList();
                return Ok(opportunityDtos);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET api/Opportunity/Impulsed
        [HttpGet("Impulsed")]
        public async Task<ActionResult<IEnumerable<Opportunity>>> GetAllImpulsedOpportunities()
        {
            if (_context == null)
                return NotFound("DB context missing");

            var opportunityModels = await _context.Opportunities
                .Where(o => o.IsImpulsed == true)
                .Include(o => o.OpportunityImgs)
                .ToListAsync();

            if (!opportunityModels.Any())
            {
                return NotFound("No Opportunities were found.");
            }
            try
            {
                var opportunityDtos = opportunityModels.Select(OpportunityMapper.MapToDto).ToList();
                return Ok(opportunityDtos);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET api/Opportunity/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Opportunity>> GetOpportunityById(int id)
        {
            if (_context == null)
                return NotFound("DB context missing");

            if (id <= 0)
            {
                return BadRequest("Given opportunityId is invalid, it should be greater than 0.");
            }

            var opportunity = await _context.Opportunities
                .Include(o => o.OpportunityImgs)
                .FirstOrDefaultAsync(o => o.OpportunityId == id);

            if (opportunity == null)
            {
                return NotFound($"Opportunity with id {id} not found.");
            }
            try
            {
                var opportunityDto = OpportunityMapper.MapToDto(opportunity);
                return Ok(opportunityDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET api/Opportunity/User/1
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Opportunity>>> GetAllOpportunitiesByUserId(int userId)
        {
            if (_context == null)
                return NotFound("DB context missing");

            if (userId <= 0)
            {
                return BadRequest("Given userId is invalid, it should be greater than 0.");
            }

            var opportunities = await _context.Opportunities
                .Where(e => e.UserID == userId)
                .Include(o => o.OpportunityImgs)
                .ToListAsync();

            if (!opportunities.Any())
            {
                return NotFound($"Opportunity with userId {userId} not found.");
            }
            try
            {
                var opportunityDtos = opportunities.Select(OpportunityMapper.MapToDto).ToList();
                return Ok(opportunityDtos);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // GET api/Opportunity/Search?keyword=event&vacancies=5&minPrice=10&maxPrice=100&category=conference&location=VilaReal
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
            if (_context == null)
                return NotFound("DB context missing");

            var errors = OpportunityService.ValidateSearchParameters(vacancies, minPrice, maxPrice, category, location);
            if (errors.Any())
            {
                return BadRequest(string.Join("; ", errors));
            }

            var query = _context.Opportunities.AsQueryable();

            // Apply filters based on provided parameters
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(o => o.Name.Contains(keyword) || (o.Description != null && o.Description.Contains(keyword)));
            if (vacancies.HasValue)
                query = query.Where(o => o.Vacancies >= vacancies.Value);

            if (minPrice.HasValue)
                query = query.Where(o => o.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(o => o.Price <= maxPrice.Value);

            if (category.HasValue)
                query = query.Where(o => o.Category == category.Value);

            if (location.HasValue)
                query = query.Where(o => o.Location == location.Value);


            // Execute the query and return the results

            var opportunitiesModels = await query
                .Include(o => o.OpportunityImgs)
                .ToListAsync();
            try
            {
                var opportunityDtos = opportunitiesModels.Select(OpportunityMapper.MapToDto).ToList();
                return Ok(opportunityDtos);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //POST api/Opportunity/
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<Opportunity>> CreateOpportunity(Opportunity opportunity)
        {
            if (_context == null)
                return NotFound("DB context missing");

            var errors = OpportunityService.ValidateOpportunityParameters(
                opportunity.name,
                opportunity.description,
                opportunity.price,
                opportunity.vacancies,
                opportunity.category,
                opportunity.location,
                opportunity.address,
                opportunity.date,
                true // This is a creation request
            );

            if (errors.Any())
            {
                return BadRequest(string.Join("; ", errors));
            }

            var userExists = await _context.Users.AnyAsync(u => u.UserId == opportunity.userId);
            if (!userExists)
            {
                return BadRequest("Invalid User ID. User does not exist.");
            }

            try
            {
                // Initialize the review score
                opportunity.reviewScore = 0.0F;

                // Map DTO to model
                var opportunityModel = OpportunityMapper.MapToModel(opportunity);

                // If there are images in the opportunity, map them to the model
                if (opportunity.OpportunityImgs != null && opportunity.OpportunityImgs.Any())
                {
                    opportunityModel.OpportunityImgs = opportunity.OpportunityImgs
                        .Select(OpportunityImgMapper.MapToModel) // Assuming you have a mapper for images
                        .ToList();
                }

                // Add the opportunity model to the context
                await _context.Opportunities.AddAsync(opportunityModel);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Map the created model back to DTO
                var createdOpportunityDto = OpportunityMapper.MapToDto(opportunityModel);

                // Return the created opportunity
                return CreatedAtAction(nameof(GetOpportunityById), new { id = createdOpportunityDto.opportunityId }, createdOpportunityDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //DELETE api/Opportunity/
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Opportunity>> DeleteOpportunityById(int id)
        {
            if (_context == null)
                return NotFound("DB context missing");

            if (id <= 0)
            {
                return BadRequest("Given opportunityId is invalid, it should be greater than 0.");
            }

            var opportunityModel = await _context.Opportunities.FindAsync(id);

            if (opportunityModel == null)
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
            if (_context == null)
                return NotFound("DB context missing");

            if (id <= 0)
            {
                return BadRequest("Given opportunityId is invalid, it should be greater than 0.");
            }

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

            }
            else
            {
                return BadRequest("Opportunity is Already Active");
            }

        }

        //PUT api/Opportunity/1/deactivate
        [HttpPut("{id}/deactivate")]
        [Authorize]
        public async Task<ActionResult<Opportunity>> DeactivateOpportunityById(int id)
        {
            if (_context == null)
                return NotFound("DB context missing");

            if (id <= 0)
            {
                return BadRequest("Given opportunityId is invalid, it should be greater than 0.");
            }

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

            }
            else
            {
                return BadRequest("Opportunity is Already Inactive");
            }

        }

        // PUT api/Opportunity/1/Edit?name=event&description=description&price=10&vacancies=2&category=agricultura&location=VilaReal&address=RuaTeste&date=10/02/2025&newImages=["img1","img2"]
        [HttpPut("{id}/Edit")]
        [Authorize]
        public async Task<ActionResult<Opportunity>> EditOpportunityById(
            int id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] decimal? price,
            [FromQuery] int? vacancies,
            [FromQuery] Category? category,
            [FromQuery] Location? location,
            [FromQuery] string? address,
            [FromQuery] DateTime? date,
            [FromBody] List<byte[]>? newImageUrls
        )
        {
            if (_context == null)
                return NotFound("DB context missing");

            if (id <= 0)
            {
                return BadRequest("Given opportunityId is invalid, it should be greater than 0.");
            }

            var opportunityModel = await _context.Opportunities
                .Include(o => o.OpportunityImgs)
                .FirstOrDefaultAsync(o => o.OpportunityId == id);

            if (opportunityModel == null)
            {
                return BadRequest($"Opportunity with id {id} not found.");
            }

            var errors = OpportunityService.ValidateOpportunityParameters(
                name,
                description,
                price,
                vacancies,
                category,
                location,
                address,
                date,
                false // Indicate that is a edit and not creating request
            );

            if (errors.Any())
            {
                return BadRequest(string.Join("; ", errors));
            }

            // Update fields only if they have valid values
            if (!string.IsNullOrEmpty(name)) opportunityModel.Name = name;
            if (!string.IsNullOrEmpty(description)) opportunityModel.Description = description;
            if (price.HasValue) opportunityModel.Price = price.Value;
            if (vacancies.HasValue) opportunityModel.Vacancies = vacancies.Value;
            if (category.HasValue) opportunityModel.Category = category.Value;
            if (location.HasValue) opportunityModel.Location = location.Value;
            if (!string.IsNullOrEmpty(address)) opportunityModel.Address = address;
            if (date.HasValue) opportunityModel.Date = date.Value;

            if (newImageUrls != null)
            {
                // Remove existing images
                _context.OpportunityImgs.RemoveRange(opportunityModel.OpportunityImgs);

                // Add new images
                var newImages = newImageUrls.Select(url => new OpportunityImgModel
                {
                    Image = url,
                    OpportunityId = opportunityModel.OpportunityId
                }).ToList();

                await _context.OpportunityImgs.AddRangeAsync(newImages);
            }


            await _context.SaveChangesAsync();
            try
            {
                var opportunityDto = OpportunityMapper.MapToDto(opportunityModel);
                return Ok(opportunityDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
