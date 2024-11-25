using BackEnd.GenericClasses;
using BackEnd.Models.FrontEndModels;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Interfaces
{

    public interface IUserService
    {
        Task<ServiceResponse<User>> GetUserByIDAsync(int id);

        Task<ServiceResponse<User>> CreateNewUserAsync(User user);

        Task<ServiceResponse<string>> DeleteUserAsync(int id);

        Task<ServiceResponse<User>> EditUserAsync(int id, User updatedUser);

        Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request);

        Task<ServiceResponse<bool>> CheckEmailAvailabilityAsync(string email);

        Task<ServiceResponse<string>> ActivateAccountAsync(string token);

        Task<ServiceResponse<Impulse>> ImpulseOpportunityAsync(Impulse impulse);

    }
}
