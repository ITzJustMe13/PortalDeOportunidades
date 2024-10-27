using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Models.Mappers
{
    public class UserMapper
    {
        // Method to map OpportunityModel to Opportunity(Dto)
        public static User MapToDto(UserModel userModel)
        {
            if (userModel == null)
            {
                return null;
            }
            return new User
            {
                userId = userModel.UserId,
                firstName = userModel.FirstName,
                lastName = userModel.LastName,
                email = userModel.Email,
                cellPhoneNumber = userModel.CellPhoneNum,
                regsitrationDate = userModel.RegistrationDate,
                birthDate = userModel.BirthDate,
                gender = userModel.Gender
            };
        }

        // Method to map Opportunity(Dto) to OpportunityModel
        public static UserModel MapToModel(User user)
        {
            if (user == null)
            {
                return null;
            }
            return new UserModel
            {
                UserId = user.userId,
                FirstName = user.firstName,
                LastName = user.lastName,
                Email = user.email,
                CellPhoneNum = user.cellPhoneNumber,
                RegistrationDate = user.regsitrationDate,
                BirthDate = user.birthDate,
                Gender = user.gender,
            };
        }
    }
}