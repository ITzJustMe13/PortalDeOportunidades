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

        /// <summary>
        /// Endpoint that gets all the Opportunities
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that gets all the Impulsed Opportunities
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that gets an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that gets all the Opportunities of a certain user by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that creates an Opportunity
        /// </summary>
        /// <param name="opportunity"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that deletes an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that activates an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint that deactivates an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
