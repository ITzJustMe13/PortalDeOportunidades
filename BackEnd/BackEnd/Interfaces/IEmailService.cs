using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Interfaces
{
    /// <summary>
    /// This interface is responsibile for all the functions of the logic part of EmailService
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the activation email to the user with a link to activate the account
        /// </summary>
        /// <param name="user"></param>
        void SendActivationEmail(UserModel user);

        /// <summary>
        /// Sends a Information Email to the owner of the Opportunity informing him that his opportunity was reserved
        /// </summary>
        /// <param name="user"></param>
        /// <param name="opportunity"></param>
        void SendReservationEmailOppOwner(UserModel user, OpportunityModel opportunity, ReservationModel reservation);

        /// <summary>
        /// Sends a Information Email to the user informing about his reservation
        /// </summary>
        /// <param name="user"></param>
        /// <param name="reservation"></param>
        void SendReservationEmailCustomer(UserModel user, Reservation reservation);
    }
}
