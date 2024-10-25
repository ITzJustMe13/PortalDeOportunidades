using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.BackEndModels
{
    [PrimaryKey(nameof(UserId), nameof(OpportunityId))]

    public class ImpulseModel
    {
        public int UserId { get; set; }

        public virtual required UserModel User { get; set; }

        public int OpportunityId { get; set; }

        public virtual required OpportunityModel Opportunity { get; set; }

    }
}
