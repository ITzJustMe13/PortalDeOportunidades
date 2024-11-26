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
    public class ReviewController : BaseController
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

            return HandleResponse(serviceResponse);
        }

        //POST api/Review
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview(Review review)
        {
            var serviceResponse = await _reviewService.CreateReviewAsync(review);

            if (!serviceResponse.Success)
            {
                return HandleResponse(serviceResponse);
            }

            return HandleCreatedAtAction(serviceResponse, nameof(GetReviewById), new {id = serviceResponse.Data.reservationId});
        }

        //DELETE api/Review/1
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReviewById(int id)
        {
            var serviceResponse = await _reviewService.DeleteReviewByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        //PUT api/Review/1/Edit?score=2.5&desc=teste123
        [HttpPut("{id}/Edit")]
        [Authorize]
        public async Task<IActionResult> EditReviewById(int id, Review updatedReview)
        {
            var serviceResponse = await _reviewService.EditReviewByIdAsync(id, updatedReview);

            return HandleResponse(serviceResponse);
        }

    }
}
