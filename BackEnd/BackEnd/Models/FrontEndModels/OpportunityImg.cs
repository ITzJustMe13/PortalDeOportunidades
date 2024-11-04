namespace BackEnd.Models.FrontEndModels
{
    public class OpportunityImg
    {
        public int imgId { get; set; }
        public int opportunityId { get; set; }
        public required byte[] image { get; set; }
    }
}
