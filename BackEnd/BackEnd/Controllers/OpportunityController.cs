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
    public class OpportunityController : BaseController
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
        public async Task<IActionResult> GetOpportunityById(int id)
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
        public async Task<IActionResult> CreateOpportunity(Opportunity opportunity)
        {
            var serviceResponse = await _opportunityService.CreateOpportunityAsync(opportunity);

            if (!serviceResponse.Success)
            {
                return HandleResponse(serviceResponse);
            }

            return HandleCreatedAtAction(serviceResponse,
                nameof(GetOpportunityById),
                new { id = serviceResponse.Data.opportunityId }        
            );
        }

        //DELETE api/Opportunity/1
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOpportunityById(int id)
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
        public async Task<IActionResult> EditOpportunityById(
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

            return HandleResponse(serviceResponse);
        }
    }
}
