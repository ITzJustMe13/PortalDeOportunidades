using BackEnd.Controllers.Data;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
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
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext reviewContext) => this._context = reviewContext;

        /// <summary>
        /// Endpoint that gets the specific Review by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Review>> GetReviewById(int id)
        {

            if (_context == null)
                return NotFound("DB context missing");

            var reviewModel = await _context.Reviews.FindAsync(id);
            if (reviewModel == null)
            {
                return NotFound($"Review with id {id} not found.");
            }
            try
            {
                var reviewDto = ReviewMapper.MapToDto(reviewModel);
                return Ok(reviewDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint that creates a review
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Review>> CreateReview(Review review)
        {
            if (_context == null)
                return NotFound("DB context missing");

            if (review == null)
            {
                return BadRequest("Review data is required.");
            }
            if(review.reservationId == null || review.reservationId <= 0)
            {
                return BadRequest("Invalid Review id");
            }
            if (review.rating < 0 || review.rating > 5)
            {
                return BadRequest("Rating can't be below 0 or bigger than 5");
            }
            var reservationExists = await _context.Reservations.AnyAsync(r => r.reservationID == review.reservationId);
            if (!reservationExists)
            {
                return BadRequest("Invalid Reservation ID. Reservation does not exist.");
            }

            var reviewExists = await _context.Reviews.AnyAsync(r => r.ReservationId == review.reservationId);
            if (reviewExists)
            {
                return BadRequest("A review for this reservation already exists.");
            }

            try
            {
                //Creates review to DB
                var reviewModel = ReviewMapper.MapToModel(review);
                await _context.Reviews.AddAsync(reviewModel);
                await _context.SaveChangesAsync();

                //Go get the reservation of the review for the OppID
                var reservation = await _context.Reservations
                    .Include(r => r.Opportunity)
                    .FirstOrDefaultAsync(r => r.reservationID == review.reservationId);

                if (reservation?.Opportunity == null)
                {
                    return BadRequest("Reservation or associated Opportunity not found.");
                }

                // Calculate the new average score for the opportunity
                var opportunityId = reservation.Opportunity.OpportunityId;
                var averageScore = await _context.Reviews
                    .Where(r => r.Reservation.opportunityID == opportunityId)
                    .AverageAsync(r => (float?)r.Rating) ?? 0.0F; //Calculates the avg score of all the reviews and takes care of when there is no review

                // Update the opportunity review score
                reservation.Opportunity.Score = averageScore;
                await _context.SaveChangesAsync();


                var createdReviewDto = ReviewMapper.MapToDto(reviewModel);
                return CreatedAtAction(nameof(GetReviewById), new { id = createdReviewDto.reservationId }, createdReviewDto);
            }
            catch (ValidationException ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint that deletes a review by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Review>> DeleteReviewById(int id)
        {
            if (_context == null)
                return NotFound("DB context missing");

            var reviewModel = await _context.Reviews.FindAsync(id);

            if(reviewModel == null)
            {
                return NotFound($"Review with id {id} not found.");
            }

            _context.Reviews.Remove(reviewModel);
            await _context.SaveChangesAsync();

            return NoContent();
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
        public async Task<ActionResult<Review>> EditReviewById(int id, [FromQuery]float score, [FromQuery]string? desc)
        {
            if (_context == null)
                return NotFound("DB context missing");

            var reviewModel = await _context.Reviews.FindAsync(id);

            if(reviewModel == null)
            {
                return NotFound($"Review with id {id} not found.");
            }

            if (!string.IsNullOrWhiteSpace(desc))
            {
                reviewModel.Desc = desc;
            }
            if (score >= 0 || score > 5) 
            {
                reviewModel.Rating = score;
            } else
            {
                return BadRequest("Rating can't be below 0 or bigger than 5");
            }

            await _context.SaveChangesAsync();
            try
            {
                var reviewDto = ReviewMapper.MapToDto(reviewModel);
                return Ok(reviewDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
