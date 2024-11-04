using BackEnd.Models.BackEndModels;

namespace BackEnd.Services
{
    public class EmailService
    {
        public void SendActivationEmail(UserModel user)
        {
           

            var activationLink = $"https://yourdomain.com/activate?token={user.Token}";
            // Send email logic here using SMTP or email service like SendGrid, etc.
        }
    }
}
