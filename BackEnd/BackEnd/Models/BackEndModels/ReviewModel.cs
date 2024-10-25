using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.BackEndModels
{
    public class ReviewModel
    {
        [Key]
        public int ReservationId { get; set; }

        [Required]
        public int Rating {  get; set; }


        [StringLength(1000)]
        public string? Desc { get; set; }

        public ICollection<ReservationModel> Reservations { get; set; }
    }
}
