﻿using BackEnd.Models.BackEndModels;
using System.Net.Mail;
using System.Net;
using BackEnd.Interfaces;

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
            var fromPassword = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");
            var activationLink = $"https://localhost:7235/api/User/activate?token={user.Token}";
            var fromAddress = new MailAddress("portaldeoportunidades2024@gmail.com", "Mail");
            var toAddress = new MailAddress(user.Email);
            const string subject = "Activate Your Account";
            string body = $"Hello {user.FirstName} {user.LastName},\n\nPlease click the link below to activate your account:\n{activationLink}\n\nThank you!";

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
