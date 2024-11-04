using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.BackEndModels
{
    [PrimaryKey(nameof(UserId), nameof(OpportunityId))]
    public class FavoritesModel
    {
        [ForeignKey("UserModel")]
        public int UserId { get; set; }
        public  UserModel? User { get; set; }

        [ForeignKey("OpportunityModel")]
        public int OpportunityId { get; set; }

        public OpportunityModel? Opportunity { get; set; }
    }
}
