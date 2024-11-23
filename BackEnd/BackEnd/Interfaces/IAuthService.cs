using BackEnd.Models.BackEndModels;

namespace BackEnd.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(UserModel user);
    }
}
