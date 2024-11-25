using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Models.Mappers
{
    public class UserMapper
    {
        /// <summary>
        /// Function that maps UserModel parameters to User parameters
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns>Returns a Dto with the Model info</returns>
        public static User? MapToDto(UserModel userModel)
        {
            if (userModel == null)
            {
                return null;
            }

            // Preenchimento do campo RegistrationDate se não estiver definido
            var registrationDate = userModel.RegistrationDate == default ? DateTime.Now : userModel.RegistrationDate;

            // Mapear campos
            return new User
            {
                userId = userModel.UserId,
                firstName = userModel.FirstName,
                lastName = userModel.LastName,
                password = null,
                email = userModel.Email,
                cellPhoneNumber = userModel.CellPhoneNum,
                registrationDate = registrationDate,
                birthDate = userModel.BirthDate,
                gender = userModel.Gender,
                image = userModel.Image,
                IBAN = userModel.IBAN
            };
        }

        /// <summary>
        /// Function that maps User parameters to UserModel parameters
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns a Model with the Dto info</returns>
        public static UserModel? MapToModel(User user)
        {
            if (user == null)
            {
                return null;
            }

            // Preencher automaticamente a data de registro se não estiver definida
            if (user.registrationDate == default)
            {
                user.registrationDate = DateTime.Now;
            }


            var userModel = new UserModel
            {
                UserId = user.userId,
                FirstName = user.firstName!,
                LastName = user.lastName!,
                HashedPassword = user.password,
                Email = user.email!,
                CellPhoneNum = (int)user.cellPhoneNumber!,
                RegistrationDate = user.registrationDate,
                BirthDate = (DateTime)user.birthDate!,
                Gender = (Enums.Gender)user.gender!,
                Image = user.image,
                IBAN = user.IBAN
            };
                ValidateModel(userModel);

            return userModel;
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
