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
    public class ReviewController : BaseCrudController<Review>
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService) 
        {
            _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
        }

        /// <summary>
        /// Endpoint that gets the specific Review by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public override async Task<IActionResult> GetEntityById(int id)
        {
            var serviceResponse = await _reviewService.GetReviewByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that creates a review
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public override async Task<IActionResult> CreateEntity(Review review)
        {
            var serviceResponse = await _reviewService.CreateReviewAsync(review);

            if (!serviceResponse.Success)
            {
                return HandleResponse(serviceResponse);
            }

            return HandleCreatedAtAction(serviceResponse, nameof(GetEntityById), new {id = serviceResponse.Data.reservationId});
        }

        /// <summary>
        /// Endpoint that deletes a review by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public override async Task<IActionResult> DeleteEntity(int id)
        {
            var serviceResponse = await _reviewService.DeleteReviewByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that edits a review by it id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="score"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        [HttpPut("{id}/Edit")]
        [Authorize]
        public override async Task<IActionResult> UpdateEntity(int id, Review updatedReview)
        {
            var serviceResponse = await _reviewService.EditReviewByIdAsync(id, updatedReview);

            return HandleResponse(serviceResponse);
        }

    }
}
