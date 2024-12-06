using BackEnd.Models.BackEndModels;
using System.Net.Mail;
using System.Net;
using BackEnd.Interfaces;
using BackEnd.Models.FrontEndModels;
using Stripe;

namespace BackEnd.Services
{
    /// <summary>
    /// This class is responsible for the email management logic of the program
    /// </summary>
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Function that sends an Activation Email to the user's email
        /// </summary>
        /// <param name="user"></param>
        public void SendActivationEmail(UserModel user)
        {
            var activationLink = $"https://portaldeoportunidades-5ddcb.web.app/#/activate-account?token={user.Token}";
            string userEmail = user.Email;
            const string subject = "Activate Your Account";
            string body = $"Olá {user.FirstName} {user.LastName},\n\nPor Favor clica no link abaixo para ativares a tua conta:\n{activationLink}\n\nObrigado!";

            SendEmail(subject, body, userEmail);
        }

        /// <summary>
        /// Sends a Information Email to the user informing about his reservation
        /// </summary>
        /// <param name="user"></param>
        /// <param name="reservation"></param>
        public void SendReservationEmailCustomer(UserModel user, Reservation reservation)
        {
            string userEmail = user.Email; 
            const string subject = "Reserva Portal de Oportunidades";
            string body = $"Olá {user.FirstName} {user.LastName},\n\n:A tua reserva para o dia {reservation.date} foi confirmada\nPara Cancelamentos e Devoluções por favor contacta o nosso suporte em:portaldeoportunidades2024@gmail.com \nObrigado!";
            SendEmail(subject, body, userEmail);    
        }

        public void SendReservationEmailOppOwner(UserModel user, OpportunityModel opportunity, ReservationModel reservation)
        {
            string userEmail = user.Email;
            const string subject = "Reserva Portal de Oportunidades";
            string body = $"Olá {user.FirstName} {user.LastName},\n\n:A tua opportunidade {opportunity.Name} para o dia {opportunity.Date} foi reservada para {reservation.numOfPeople} pessoas restam ainda {opportunity.Vacancies} vagas livres.\nPara reclamações ou informações por favor contacta o nosso suporte em:portaldeoportunidades2024@gmail.com \nObrigado!";
            SendEmail(subject, body, userEmail);
        }

        /// <summary>
        /// Sends a Information Email to the owner of the Opportunity informing him that his opportunity was reserved
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="adress"></param>
        private void SendEmail(string subject, string body, string adress)
        {
            var fromPassword = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");
            var fromAddress = new MailAddress("portaldeoportunidades2024@gmail.com", "Mail");
            var toAddress = new MailAddress(adress);
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
