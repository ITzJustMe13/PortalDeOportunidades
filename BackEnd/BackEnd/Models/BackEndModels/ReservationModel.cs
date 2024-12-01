using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.BackEndModels
{
    public class ReservationModel
    {

        [Key]
        public int reservationID {  get; set; }

        [ForeignKey("OpportunityModel")]
        public int opportunityID { get; set; }
        public virtual OpportunityModel Opportunity { get; set; }

        [ForeignKey("UserModel")]
        public int userID { get; set; }
        public virtual UserModel User { get; set; }

        [Required(ErrorMessage = "O campo 'reservationDate' é obrigatório.")]
        public DateTime reservationDate { get; set; }

        [Required(ErrorMessage = "O campo 'checkInDate' é obrigatório.")]
        public DateTime checkInDate { get; set; }

        [Required(ErrorMessage = "O campo 'numOfPeople' é obrigatório.")]
        [Range(1, 10, ErrorMessage = "O 'numOfPeople' deve ter um valor de 1 a 10")]
        public int numOfPeople { get; set; }

        [Required(ErrorMessage = "O campo 'isActive' é obrigatório.")]
        public bool isActive  { get; set; }

        [Required(ErrorMessage = "O campo 'fixedPrice' é obrigatório.")]
        public float fixedPrice { get; set; }

        public virtual ReviewModel review { get; set; }

    }
}
