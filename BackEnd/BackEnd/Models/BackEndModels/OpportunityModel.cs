using BackEnd.Enums;
using System.ComponentModel.DataAnnotations;

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
        [Range(2,6)]
        public float Price { get; set; }

        [Required]
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

    }
}
