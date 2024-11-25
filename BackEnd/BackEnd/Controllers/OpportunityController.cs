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
using BackEnd.Interfaces;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpportunityController : ControllerBase
    {
        private IOpportunityService _opportunityService;

        public OpportunityController(IOpportunityService opportunityService)
        {
            _opportunityService = opportunityService ?? throw new ArgumentNullException(nameof(opportunityService));
        }

        //GET api/Opportunity/
        [HttpGet]
        public async Task<ActionResult> GetAllOpportunities()
        {
            var serviceResponse = await _opportunityService.GetAllOpportunitiesAsync();

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest( serviceResponse.Message),
                    _ => StatusCode(500, serviceResponse.Message ) // Default for unexpected errors
                };
            }

            return Ok(serviceResponse.Data);
        }

        //GET api/Opportunity/Impulsed
        [HttpGet("Impulsed")]
        public async Task<ActionResult> GetAllImpulsedOpportunities()
        {
            var serviceResponse = await _opportunityService.GetAllImpulsedOpportunitiesAsync();

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message ),
                    _ => StatusCode(500, serviceResponse.Message) // Default for unexpected errors
                };
            }

            return Ok(serviceResponse.Data);
        }

        //GET api/Opportunity/1
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOpportunityById(int id)
        {
            var serviceResponse = await _opportunityService.GetOpportunityByIdAsync(id);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message ),
                    _ => StatusCode(500,  serviceResponse.Message ) // InternalServerError
                };
            }

            return Ok(serviceResponse.Data);
        }

        //GET api/Opportunity/User/1
        [HttpGet("User/{userId}")]
        public async Task<ActionResult> GetAllOpportunitiesByUserId(int userId)
        {
            var serviceResponse = await _opportunityService.GetAllOpportunitiesByUserIdAsync(userId);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message ),
                    _ => StatusCode(500,  serviceResponse.Message) // InternalServerError
                };
            }

            return Ok(serviceResponse.Data);
        }

        // GET api/Opportunity/Search?keyword=event&vacancies=5&minPrice=10&maxPrice=100&category=conference&location=VilaReal
        [HttpGet("Search")]
        public async Task<ActionResult> SearchOpportunities(
            [FromQuery] string? keyword,
            [FromQuery] int? vacancies,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] Category? category,
            [FromQuery] Location? location
        )
        {
            var serviceResponse = await _opportunityService.SearchOpportunitiesAsync(keyword, vacancies, minPrice, maxPrice, category, location);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest( serviceResponse.Message ),
                    _ => StatusCode(500, serviceResponse.Message) // InternalServerError
                };
            }

            return Ok(serviceResponse.Data);
        }

        //POST api/Opportunity/
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateOpportunity(Opportunity opportunity)
        {
            var serviceResponse = await _opportunityService.CreateOpportunityAsync(opportunity);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    _ => StatusCode(500,  serviceResponse.Message) // InternalServerError
                };
            }

            return CreatedAtAction(
                nameof(GetOpportunityById),
                new { id = serviceResponse.Data.opportunityId },
                serviceResponse.Data
            );
        }

        //DELETE api/Opportunity/1
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteOpportunityById(int id)
        {
            var serviceResponse = await _opportunityService.DeleteOpportunityByIdAsync(id);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    _ => StatusCode(500,  serviceResponse.Message) // InternalServerError
                };
            }

            return NoContent();
        }

        //PUT api/Opportunity/1/activate
        [HttpPut("{id}/activate")]
        [Authorize]
        public async Task<ActionResult> ActivateOpportunityById(int id)
        {
            var serviceResponse = await _opportunityService.ActivateOpportunityByIdAsync(id);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    _ => StatusCode(500,  serviceResponse.Message) // InternalServerError
                };
            }

            return NoContent();
        }

        //PUT api/Opportunity/1/deactivate
        [HttpPut("{id}/deactivate")]
        [Authorize]
        public async Task<ActionResult<Opportunity>> DeactivateOpportunityById(int id)
        {
            var serviceResponse = await _opportunityService.DeactivateOpportunityByIdAsync(id);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    _ => StatusCode(500,  serviceResponse.Message) // InternalServerError
                };
            }

            return NoContent();
        }

        // PUT api/Opportunity/1/Edit?name=event&description=description&price=10&vacancies=2&category=agricultura&location=VilaReal&address=RuaTeste&date=10/02/2025&newImages=["img1","img2"]
        [HttpPut("{id}/Edit")]
        [Authorize]
        public async Task<ActionResult> EditOpportunityById(
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
            var serviceResponse = await _opportunityService.EditOpportunityByIdAsync(
                id,
                name,
                description,
                price,
                vacancies,
                category,
                location,
                address,
                date,
                newImageUrls
            );

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    _ => StatusCode(500, serviceResponse.Message)
                };
            }

            return Ok(serviceResponse.Data); // Return the updated opportunity DTO
        }
    }
}
