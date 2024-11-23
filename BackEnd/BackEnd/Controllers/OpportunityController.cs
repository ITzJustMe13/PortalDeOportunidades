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

        /// <summary>
        /// Endpoint that gets all the Opportunities
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that gets all the Impulsed Opportunities
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that gets an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that gets all the Opportunities of a certain user by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that searches Opportunities based on the parameteres given
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="vacancies"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that creates an Opportunity
        /// </summary>
        /// <param name="opportunity"></param>
        /// <returns></returns>
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
                // Initialize review score
                opportunity.reviewScore = 0.0F;

                var opportunityModel = OpportunityMapper.MapToModel(opportunity);

                // If the opportunity has imgs map them to the model
                if (opportunity.OpportunityImgs != null && opportunity.OpportunityImgs.Any())
                {
                    opportunityModel.OpportunityImgs = opportunity.OpportunityImgs
                        .Select(OpportunityImgMapper.MapToModel)
                        .ToList();
                }

                await _context.Opportunities.AddAsync(opportunityModel);

                await _context.SaveChangesAsync();

                // Map the created model back to DTO
                var createdOpportunityDto = OpportunityMapper.MapToDto(opportunityModel);

                return CreatedAtAction(nameof(GetOpportunityById), new { id = createdOpportunityDto.opportunityId }, createdOpportunityDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint that deletes an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that activates an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that deactivates an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoints that edits an Opportunity by its id using the given parameters
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="price"></param>
        /// <param name="vacancies"></param>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <param name="address"></param>
        /// <param name="date"></param>
        /// <param name="newImageUrls"></param>
        /// <returns></returns>
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
