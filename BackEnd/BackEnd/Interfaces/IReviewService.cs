
using BackEnd.Models.FrontEndModels;
using BackEnd.ServiceResponses;

namespace BackEnd.Interfaces
{
    public interface IReviewService
    {
        Task<ServiceResponse<Review>> GetReviewByIdAsync(int id);

        Task<ServiceResponse<Review>> CreateReviewAsync(Review review);

        Task<ServiceResponse<bool>> DeleteReviewByIdAsync(int id);

        Task<ServiceResponse<Review>> EditReviewByIdAsync(int id, float? score, string? desc);
    }
}
