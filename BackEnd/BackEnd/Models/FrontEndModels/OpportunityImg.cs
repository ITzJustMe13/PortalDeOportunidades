namespace BackEnd.Models.FrontEndModels
{
    public class OpportunityImg
    {
        public required int ImgId { get; set; }
        public required int OpportunityId { get; set; }
        public required byte[] Image { get; set; }
    }
}
