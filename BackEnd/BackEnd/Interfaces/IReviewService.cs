
using BackEnd.Models.FrontEndModels;
using BackEnd.ServiceResponses;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Interfaces
{
    /// <summary>
    /// This interface is responsibile for all the functions of the logic part of Review
    /// </summary>
    public interface IReviewService
    {
        Task<ServiceResponse<Review>> GetReviewByIdAsync(int id);

        Task<ServiceResponse<Review>> CreateReviewAsync(Review review);

        Task<ServiceResponse<bool>> DeleteReviewByIdAsync(int id);

        Task<ServiceResponse<Review>> EditReviewByIdAsync(int id, Review updatedReview);

        Task<ServiceResponse<IEnumerable<Review>>> GetReviewsByUserAsync(int userId);
    }
}
