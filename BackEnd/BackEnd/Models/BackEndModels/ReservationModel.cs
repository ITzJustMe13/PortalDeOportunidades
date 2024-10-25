using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.BackEndModels
{
    public class ReservationModel
    {

        [Key]
        public int reservationID {  get; set; }

        [ForeignKey("opportunityID")]
        public int opportunityID { get; set; }
    }
}
