using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Mappers
{
    public class ReviewMapper
    {
        // Method to map ReviewModel to Review(Dto)
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

        // Method to map Review(Dto) to ReviewModel
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

        // Método para validar o modelo através das DataAnnotations
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
