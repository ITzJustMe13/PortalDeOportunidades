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

        //GET api/Opportunity/
        [HttpGet]
        public async Task<IActionResult> GetAllOpportunities()
        {
            var serviceResponse = await _opportunityService.GetAllOpportunitiesAsync();

            return HandleResponse(serviceResponse);
        }

        //GET api/Opportunity/Impulsed
        [HttpGet("Impulsed")]
        public async Task<IActionResult> GetAllImpulsedOpportunities()
        {
            var serviceResponse = await _opportunityService.GetAllImpulsedOpportunitiesAsync();

            return HandleResponse(serviceResponse);
        }

        //GET api/Opportunity/1
        [HttpGet("{id}")]
        public override async Task<IActionResult> GetEntityById(int id)
        {
            var serviceResponse = await _opportunityService.GetOpportunityByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        //GET api/Opportunity/User/1
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetAllOpportunitiesByUserId(int userId)
        {
            var serviceResponse = await _opportunityService.GetAllOpportunitiesByUserIdAsync(userId);

            return HandleResponse(serviceResponse);
        }

        // GET api/Opportunity/Search?keyword=event&vacancies=5&minPrice=10&maxPrice=100&category=conference&location=VilaReal
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

        //POST api/Opportunity/
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

        //DELETE api/Opportunity/1
        [HttpDelete("{id}")]
        [Authorize]
        public override async Task<IActionResult> DeleteEntity(int id)
        {
            var serviceResponse = await _opportunityService.DeleteOpportunityByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        //PUT api/Opportunity/1/activate
        [HttpPut("{id}/activate")]
        [Authorize]
        public async Task<IActionResult> ActivateOpportunityById(int id)
        {
            var serviceResponse = await _opportunityService.ActivateOpportunityByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        //PUT api/Opportunity/1/deactivate
        [HttpPut("{id}/deactivate")]
        [Authorize]
        public async Task<IActionResult> DeactivateOpportunityById(int id)
        {
            var serviceResponse = await _opportunityService.DeactivateOpportunityByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        // PUT api/Opportunity/1/Edit?name=event&description=description&price=10&vacancies=2&category=agricultura&location=VilaReal&address=RuaTeste&date=10/02/2025&newImages=["img1","img2"]
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
