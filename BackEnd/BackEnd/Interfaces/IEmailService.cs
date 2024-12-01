using BackEnd.Models.BackEndModels;

namespace BackEnd.Interfaces
{
    public interface IEmailService
    {
        void SendActivationEmail(UserModel user);
    }
}
