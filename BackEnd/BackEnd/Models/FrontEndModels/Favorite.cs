namespace BackEnd.Models.FrontEndModels
{
    public class Favorite
    {
        public int FavoriteId { get; set; }

        public User User { get; set; }

        public Opportunity Opportunity { get; set; }
    }
}
