using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.BackEndModels
{
    public class OpportunityImgModel
    {
        [Key]
        public int ImgId { get; set; }

        [Required]
        public int OpportunityId { get; set; }

        [Required]
        public byte[] Image { get; set; }

        public virtual OpportunityModel Opportunity { get; set; }
    }
}
