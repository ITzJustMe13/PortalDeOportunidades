using BackEnd.Enums;
using BackEnd.Models.FrontEndModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Interfaces;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpportunityController : BaseCrudController<Opportunity>
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
        public async Task<IActionResult> GetAllOpportunities()
        {
            var serviceResponse = await _opportunityService.GetAllOpportunitiesAsync();

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that gets all the Impulsed Opportunities
        /// </summary>
        /// <returns></returns>
        [HttpGet("Impulsed")]
        public async Task<IActionResult> GetAllImpulsedOpportunities()
        {
            var serviceResponse = await _opportunityService.GetAllImpulsedOpportunitiesAsync();

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that gets an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public override async Task<IActionResult> GetEntityById(int id)
        {
            var serviceResponse = await _opportunityService.GetOpportunityByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that gets all the Opportunities of a certain user by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetAllOpportunitiesByUserId(int userId)
        {
            var serviceResponse = await _opportunityService.GetAllOpportunitiesByUserIdAsync(userId);

            return HandleResponse(serviceResponse);
        }

        [HttpGet("Reviews/{id}")]
        public async Task<IActionResult> GetAllReviewsByOpportunityId(int id)
        {
            var serviceResponse = await _opportunityService.GetAllReviewsByOpportunityIdAsync(id);

            return HandleResponse(serviceResponse);
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
        public async Task<IActionResult> SearchOpportunities(
            [FromQuery] string? keyword,
            [FromQuery] int? vacancies,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] Category? category,
            [FromQuery] Location? location
        )
        {
            var serviceResponse = await _opportunityService.SearchOpportunitiesAsync(keyword, vacancies, minPrice, maxPrice, category, location);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that creates an Opportunity
        /// </summary>
        /// <param name="opportunity"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public override async Task<IActionResult> CreateEntity(Opportunity opportunity)
        {
            var serviceResponse = await _opportunityService.CreateOpportunityAsync(opportunity);

            if (!serviceResponse.Success)
            {
                return HandleResponse(serviceResponse);
            }

            return HandleCreatedAtAction(serviceResponse,
                nameof(GetEntityById),
                new { id = serviceResponse.Data.opportunityId }        
            );
        }

        /// <summary>
        /// Endpoint that deletes an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public override async Task<IActionResult> DeleteEntity(int id)
        {
            var serviceResponse = await _opportunityService.DeleteOpportunityByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that activates an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/activate")]
        [Authorize]
        public async Task<IActionResult> ActivateOpportunityById(int id)
        {
            var serviceResponse = await _opportunityService.ActivateOpportunityByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that deactivates an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/deactivate")]
        [Authorize]
        public async Task<IActionResult> DeactivateOpportunityById(int id)
        {
            var serviceResponse = await _opportunityService.DeactivateOpportunityByIdAsync(id);

            return HandleResponse(serviceResponse);
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
        public override async Task<IActionResult> UpdateEntity(
            int id,
            Opportunity updatedOpportunity
        )
        {
            var serviceResponse = await _opportunityService.EditOpportunityByIdAsync(
                id,
                updatedOpportunity
            );

            return HandleResponse(serviceResponse);
        }
    }
}
