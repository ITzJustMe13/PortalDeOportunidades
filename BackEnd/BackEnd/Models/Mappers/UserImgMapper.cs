using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Mappers
{
    public class UserImgMapper
    {
        // Method to map UserImgModel to UserImg(Dto)
        public static UserImg MapToDto(UserImgModel userImgModel)
        {
            if (userImgModel == null)
                return null;

            ValidateModel(userImgModel);

            return new UserImg
            {
                ImgId = userImgModel.ImgId,
                UserId = userImgModel.UserId,
                Image = userImgModel.Image

            };
        }

        // Method to map UserImg(Dto) to UserImgModel
        public static UserImgModel MapToModel(UserImg userImg)
        {
            if (userImg == null)
            {
                return null;
            }

            var userImgModel = new UserImgModel
            {
                ImgId = userImg.ImgId,
                UserId = userImg.UserId,
                Image = userImg.Image
            };
            ValidateModel(userImgModel);
            return userImgModel;
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

