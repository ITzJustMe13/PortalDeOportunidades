using BackEnd.Controllers.Data;
using BackEnd.Interfaces;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService) 
        {
            _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
        }

        //GET api/Review/1
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var serviceResponse = await _reviewService.GetReviewByIdAsync(id);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    "NotFound" => NotFound(serviceResponse.Message),
                    _ => StatusCode(500, "An unexpected error occurred.")
                };
            }

            return Ok(serviceResponse.Data);
        }

        //POST api/Review
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview(Review review)
        {
            var serviceResponse = await _reviewService.CreateReviewAsync(review);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    "NotFound" => NotFound(serviceResponse.Message),
                    _ => StatusCode(500, "An unexpected error occurred.")
                };
            }

            return CreatedAtAction(nameof(GetReviewById), new { id = serviceResponse.Data.reservationId }, serviceResponse.Data);
        }

        //DELETE api/Review/1
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReviewById(int id)
        {
            var serviceResponse = await _reviewService.DeleteReviewByIdAsync(id);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    _ => StatusCode(500, "An unexpected error occurred.")
                };
            }

            return NoContent();
        }

        //PUT api/Review/1/Edit?score=2.5&desc=teste123
        [HttpPut("{id}/Edit")]
        [Authorize]
        public async Task<IActionResult> EditReviewById(int id, [FromQuery]float score, [FromQuery]string? desc)
        {
            var serviceResponse = await _reviewService.EditReviewByIdAsync(id, score, desc);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    _ => StatusCode(500, serviceResponse.Message)
                };
            }

            return Ok(serviceResponse.Data);
        }

    }
}
