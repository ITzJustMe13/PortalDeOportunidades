using BackEnd.Models.BackEndModels;
using BackEnd.ServiceResponses;

namespace BackEnd.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(UserModel user);

    }
}
