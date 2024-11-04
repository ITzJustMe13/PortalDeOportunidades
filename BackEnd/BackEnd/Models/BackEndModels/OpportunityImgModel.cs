using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.BackEndModels
{
    public class OpportunityImgModel
    {
        [Key]
        public int ImgId { get; set; }

        [Required]
        [ForeignKey("OpportunityModel")]
        public int OpportunityId { get; set; }

        [Required]
        public byte[] Image { get; set; }

        public virtual OpportunityModel Opportunity { get; set; }
    }
}
