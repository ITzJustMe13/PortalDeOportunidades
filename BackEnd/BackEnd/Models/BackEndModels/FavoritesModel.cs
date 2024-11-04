using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.BackEndModels
{
    [PrimaryKey(nameof(UserId), nameof(OpportunityId))]
    public class FavoritesModel
    {
        public int UserId { get; set; }
        public  UserModel? User { get; set; }

        public int OpportunityId { get; set; }

        public OpportunityModel? Opportunity { get; set; }
    }
}
