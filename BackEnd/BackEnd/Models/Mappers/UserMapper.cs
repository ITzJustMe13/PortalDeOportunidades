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
        // Método para mapear UserModel para User (Dto), com validação e preenchimento automático de campos
        public static User? MapToDto(UserModel userModel)
        {
            if (userModel == null)
            {
                return null;
            }

            ValidateModel(userModel);

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
                gender = userModel.Gender
            };
        }

        // Método para mapear User (Dto) para UserModel, incluindo preenchimento de campos automáticos
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
            };

            ValidateModel(userModel);

            return userModel;
        }

        // Método para validar o modelo usando DataAnnotations
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
