using BackEnd.Controllers.Data;
using BackEnd.GenericClasses;
using BackEnd.Interfaces;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext dbContext;

        public ReviewService(
            ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ServiceResponse<Review>> GetReviewByIdAsync(int id)
        {
            var response = new ServiceResponse<Review>();

            if (dbContext == null)
            {
                response.Success = false;
                response.Message = "DB context missing.";
                response.Type = "NotFound";
                return response;
            }

            if (id <= 0)
            {
                response.Success = false;
                response.Message = "Invalid review ID. It should be greater than 0.";
                response.Type = "BadRequest";
                return response;
            }

            var reviewModel = await dbContext.Reviews.FindAsync(id);
            if (reviewModel == null)
            {
                response.Success = false;
                response.Message = $"Review with id {id} not found.";
                response.Type = "NotFound";
                return response;
            }

            try
            {
                var reviewDto = ReviewMapper.MapToDto(reviewModel);
                response.Data = reviewDto;
                response.Success = true;
                response.Message = "Review retrieved successfully.";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Type = "BadRequest";
            }

            return response;
        }

        public async Task<ServiceResponse<Review>> CreateReviewAsync(Review review)
        {
            var response = new ServiceResponse<Review>();

            if (dbContext == null)
            {
                response.Success = false;
                response.Message = "DB context missing.";
                response.Type = "NotFound";
                return response;
            }

            if (review == null)
            {
                response.Success = false;
                response.Message = "Review data is required.";
                response.Type = "BadRequest";
                return response;
            }

            if (review.reservationId == null || review.reservationId <= 0)
            {
                response.Success = false;
                response.Message = "Invalid Reservation ID.";
                response.Type = "BadRequest";
                return response;
            }

            if (review.rating < 0 || review.rating > 5)
            {
                response.Success = false;
                response.Message = "Rating must be between 0 and 5.";
                response.Type = "BadRequest";
                return response;
            }

            var reservationExists = await dbContext.Reservations.AnyAsync(r => r.reservationID == review.reservationId);
            if (!reservationExists)
            {
                response.Success = false;
                response.Message = "Invalid Reservation ID. Reservation does not exist.";
                response.Type = "BadRequest";
                return response;
            }

            var reviewExists = await dbContext.Reviews.AnyAsync(r => r.ReservationId == review.reservationId);
            if (reviewExists)
            {
                response.Success = false;
                response.Message = "A review for this reservation already exists.";
                response.Type = "BadRequest";
                return response;
            }

            try
            {
                // Create the review in the database
                var reviewModel = ReviewMapper.MapToModel(review);
                await dbContext.Reviews.AddAsync(reviewModel);
                await dbContext.SaveChangesAsync();

                // Fetch the associated reservation and opportunity
                var reservation = await dbContext.Reservations
                    .Include(r => r.Opportunity)
                    .FirstOrDefaultAsync(r => r.reservationID == review.reservationId);

                if (reservation?.Opportunity == null)
                {
                    response.Success = false;
                    response.Message = "Reservation or associated Opportunity not found.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Calculate the new average score for the opportunity
                var opportunityId = reservation.Opportunity.OpportunityId;
                var averageScore = await dbContext.Reviews
                    .Where(r => r.Reservation.opportunityID == opportunityId)
                    .AverageAsync(r => (float?)r.Rating) ?? 0.0F;

                // Update the opportunity review score
                reservation.Opportunity.Score = averageScore;
                await dbContext.SaveChangesAsync();

                // Map the created review to a DTO
                var createdReviewDto = ReviewMapper.MapToDto(reviewModel);
                response.Data = createdReviewDto;
                response.Success = true;
                response.Message = "Review created successfully.";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Type = "BadRequest";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteReviewByIdAsync(int id)
        {
            var response = new ServiceResponse<bool>();

            if (dbContext == null)
            {
                response.Success = false;
                response.Message = "DB context missing.";
                response.Type = "NotFound";
                return response;
            }

            var reviewModel = await dbContext.Reviews.FindAsync(id);

            if (reviewModel == null)
            {
                response.Success = false;
                response.Message = $"Review with id {id} not found.";
                response.Type = "NotFound";
                return response;
            }

            try
            {
                dbContext.Reviews.Remove(reviewModel);
                await dbContext.SaveChangesAsync();
                response.Success = true;
                response.Message = "Review deleted successfully.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error deleting review: {ex.Message}";
                response.Type = "InternalServerError";
            }

            return response;
        }

        public async Task<ServiceResponse<Review>> EditReviewByIdAsync(int id, float? score, string? desc)
        {
            var response = new ServiceResponse<Review>();

            if (dbContext == null)
            {
                response.Success = false;
                response.Message = "DB context missing.";
                response.Type = "NotFound";
                return response;
            }

            var reviewModel = await dbContext.Reviews.FindAsync(id);

            if (reviewModel == null)
            {
                response.Success = false;
                response.Message = $"Review with id {id} not found.";
                response.Type = "NotFound";
                return response;
            }

            if (score.HasValue)
            {
                if (score.Value < 0 || score.Value > 5)
                {
                    response.Success = false;
                    response.Message = "Rating must be between 0 and 5.";
                    response.Type = "BadRequest";
                    return response;
                }
                reviewModel.Rating = score.Value;
            }

            if (!string.IsNullOrWhiteSpace(desc))
            {
                reviewModel.Desc = desc;
            }

            await dbContext.SaveChangesAsync();

            try
            { 
                var reviewDto = ReviewMapper.MapToDto(reviewModel);
                
                response.Success = true;
                response.Data = reviewDto;
                response.Message = "Review updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error updating review: {ex.Message}";
                response.Type = "InternalServerError";
            }

            return response;
        }




    }
}
