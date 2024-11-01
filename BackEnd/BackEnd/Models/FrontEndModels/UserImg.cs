namespace BackEnd.Models.FrontEndModels
{
    public class UserImg
    {
        public required int ImgId { get; set; }
        public required int UserId { get; set; }
        public required byte[] Image { get; set; }
    }
}
