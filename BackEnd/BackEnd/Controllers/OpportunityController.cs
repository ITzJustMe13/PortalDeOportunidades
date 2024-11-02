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
            } catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            

        }

        //GET api/Opportunity/User/1
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Opportunity>>> GetAllOpportunitiesByUserId(int userId)
        {
            var opportunities = await _context.Opportunities
                .Where(e => e.User.UserId == userId)
                .Include(o => o.OpportunityImgs)
                .ToListAsync();

            

            if (opportunities == null || !opportunities.Any())
            {
                return NotFound($"Opportunity with userId {userId} not found.");
            }
            try
            {
                var opportunityDtos = opportunities.Select(OpportunityMapper.MapToDto).ToList();
                return Ok(opportunityDtos);
            } catch (ValidationException ex)
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
            var errors = ValidateSearchParameters(vacancies, minPrice, maxPrice, category, location);
            if (errors.Any())
            {
                return BadRequest(string.Join("; ", errors));
            }

            var query = _context.Opportunities.AsQueryable();

            // Apply filters based on provided parameters
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(o => o.Name.Contains(keyword) || o.Description.Contains(keyword));

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
            var errors = ValidateOpportunityParameters(
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
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
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

        // PUT api/Opportunity/1/Edit?name=event&description=description&price=10&vacancies=2&category=agricultura&location=VilaReal&address=RuaTeste&date=10/02/2025&newImages=["img1","img2"]
        [HttpPut("{id}/Edit")]
        //[Authorize]
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
            var opportunityModel = await _context.Opportunities
                .Include(o => o.OpportunityImgs)
                .FirstOrDefaultAsync(o => o.OpportunityId == id);

            if (opportunityModel == null)
            {
                return BadRequest($"Opportunity with id {id} not found.");
            }

            var errors = ValidateOpportunityParameters(
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
            if (date.HasValue) opportunityModel.date = date.Value;

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

        // Helper method for validating parameters
        private List<string> ValidateSearchParameters(int? vacancies, decimal? minPrice, decimal? maxPrice, Category? category, Location? location)
        {
            var errors = new List<string>();

            if (vacancies.HasValue && vacancies <= 0)
                errors.Add("Vacancies must be greater than zero.");

            if (minPrice.HasValue && minPrice <= 0.00M)
                errors.Add("MinPrice should be greater than 0.01.");

            if (maxPrice.HasValue && maxPrice <= 0.00M)
                errors.Add("MaxPrice should be greater than 0.01.");

            if (category.HasValue && !Enum.IsDefined(typeof(Category), category.Value))
                errors.Add("Invalid category specified.");

            if (location.HasValue && !Enum.IsDefined(typeof(Location), location.Value))
                errors.Add("Invalid location specified.");

            return errors;
        }

        private List<string> ValidateOpportunityParameters(
            string? name,
            string? description,
            decimal? price,
            int? vacancies,
            Category? category,
            Location? location,
            string? address,
            DateTime? date,
            bool isCreation // Indicate if this validation is for creating the Opp
)
        {
            var errors = new List<string>();

            if (isCreation)
            {
                if (string.IsNullOrWhiteSpace(name))
                    errors.Add("Name cannot be empty.");
                else if (name.Length > 100)
                    errors.Add("Name should be 100 characters or less.");

                if (string.IsNullOrWhiteSpace(description))
                    errors.Add("Description cannot be empty.");
                else if (description.Length > 1000)
                    errors.Add("Description should be 1000 characters or less.");
            }

            if (price.HasValue && price <= 0.00M)
                errors.Add("Price should be at least 0.01.");

            if (vacancies.HasValue && vacancies <= 0)
                errors.Add("Vacancies should be at least one.");

            if (category.HasValue && !Enum.IsDefined(typeof(Category), category.Value))
                errors.Add("Category is not valid.");

            if (location.HasValue && !Enum.IsDefined(typeof(Location), location.Value))
                errors.Add("Location is not valid.");

            if (!string.IsNullOrWhiteSpace(address) && address.Length > 200)
                errors.Add("Address should be 200 characters or less.");

            if (date.HasValue && date <= DateTime.Today)
                errors.Add("Date must be in the future.");

            return errors;
        }

    }
}
