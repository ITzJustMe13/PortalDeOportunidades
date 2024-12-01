using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.BackEndModels
{
    public class ReviewModel
    {
        [Key]
        [ForeignKey("ReservationModel")]
        public int ReservationId { get; set; }

        public virtual ReservationModel Reservation { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Rating must be 0 or higher.")]
        public float Rating {  get; set; }

        [StringLength(1000)]
        public string? Desc { get; set; }

    }
}
