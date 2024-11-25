using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Mappers
{
    /// <summary>
    /// class that maps ReviewModel to Dto and vice-versa
    /// </summary>
    public class ReviewMapper
    {
        /// <summary>
        /// Function that maps ReviewModel parameters to Review parameters
        /// </summary>
        /// <param name="reviewModel"></param>
        /// <returns>Returns a Dto with the Model info</returns>
        public static Review MapToDto(ReviewModel reviewModel)
        {
            if (reviewModel == null)
                return null;

            ValidateModel(reviewModel);

            return new Review
            {
                reservationId = reviewModel.ReservationId,
                rating = reviewModel.Rating,
                desc = reviewModel.Desc

            };
        }

        /// <summary>
        /// Function that maps Reservation parameters to ReservationModel parameters
        /// </summary>
        /// <param name="review"></param>
        /// <returns>Returns a Model with the Dto info</returns>
        public static ReviewModel MapToModel(Review review)
        {
            if (review == null)
            {
                return null;
            }

            var reviewModel = new ReviewModel
            {
                ReservationId = review.reservationId,
                Rating = review.rating,
                Desc = review.desc
            };
            ValidateModel(reviewModel);
            return reviewModel;
        }

        /// <summary>
        /// Function that validates if the mapping parameters are correct with each other
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="ValidationException"></exception>
        private static void ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, validateAllProperties: true))
            {
                // Se houver erros, lança uma exceção com detalhes
                var errorMessages = results.Select(r => r.ErrorMessage);
                throw new ValidationException("Erros de validação: " + string.Join("; ", errorMessages));
            }
        }
    }
}
