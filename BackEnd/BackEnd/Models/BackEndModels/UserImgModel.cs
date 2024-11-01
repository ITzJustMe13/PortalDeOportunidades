using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.BackEndModels
{
    public class UserImgModel
    {
        [Key]
        public int ImgId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public byte[] Image { get; set; }

        public virtual UserModel User { get; set; }
    }
}
