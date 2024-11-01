﻿using BackEnd.Controllers.Data;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
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

        //GET api/Review/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReviewById(int id)
        {
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

        //POST api/Review
        [HttpPost]
        public async Task<ActionResult<Review>> CreateReview(Review review)
        {
            if (review == null)
            {
                return BadRequest("Review data is required.");
            }
            if(review.reservationId == null)
            {
                return BadRequest("Invalid Review id");
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
                return CreatedAtAction(nameof(GetReviewById), new {id = createdReviewDto.reservationId}, createdReviewDto)
            }
            catch (ValidationException ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        //DELETE api/Review/1
        [HttpDelete("{id}")]
        public async Task<ActionResult<Review>> DeleteReviewById(int id)
        {
            var reviewModel = await _context.Reviews.FindAsync(id);

            if(reviewModel == null)
            {
                return BadRequest($"Review with id {id} not found.");
            }

            _context.Reviews.Remove(reviewModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //PUT api/Review/1/Edit?score=2.5&desc=teste123
        [HttpPut("{id}/Edit")]
        public async Task<ActionResult<Review>> EditResultById(int id, [FromQuery]float score, [FromQuery]string desc)
        {
            var reviewModel = await _context.Reviews.FindAsync(id);

            if(reviewModel == null)
            {
                return BadRequest($"Review with id {id} not found.");
            }

            if (!string.IsNullOrEmpty(desc))
            {
                reviewModel.Desc = desc;
            }
            if (score > -0.5) 
            {
                reviewModel.Rating = score;
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
