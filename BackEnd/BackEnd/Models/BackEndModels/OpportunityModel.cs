using BackEnd.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.BackEndModels
{
    public class OpportunityModel
    {

        [Key]
        public int OpportunityId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(30)]
        public int Vacancies { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public Category Category {  get; set; }

        [Required]
        public Location Location { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        public float Score { get; set; }

        [Required]
        public bool IsImpulsed { get; set; }

        public ICollection<ImpulseModel> Impulses { get; set; }

        public ICollection<FavoritesModel> Favorites { get; set; }
    }
}
