using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.BackEndModels
{
    public class ReviewModel
    {
        [Key]
        [ForeignKey("ReservationModel")]
        public int ReservationId { get; set; }

        [Required]
        public float Rating {  get; set; }

        [StringLength(1000)]
        public string? Desc { get; set; }

        public ICollection<ReservationModel> Reservations { get; set; }
    }
}
