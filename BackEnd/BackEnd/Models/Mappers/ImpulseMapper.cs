using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;

namespace BackEnd.Models.Mappers
{
    public class ImpulseMapper
    {
        // Method to map ImpulseModel to Impulse(Dto)
        public static Impulse MapToDto(ImpulseModel favoriteModel)
        {
            if (favoriteModel == null)
            {
                return null;
            }
            return new Impulse
            {
                userId = favoriteModel.UserId,
                opportunityId = favoriteModel.OpportunityId,
                value = (float)favoriteModel.Price,
                expireDate = favoriteModel.ExpireDate,
            };
        }

        // Method to map Impulse(Dto) to ImpulseModel
        public static ImpulseModel MapToModel(Impulse favorite)
        {
            if (favorite == null)
            {
                return null;
            }
            return new ImpulseModel
            {
                UserId = favorite.userId,
                OpportunityId = favorite.opportunityId,
                Price = (decimal)favorite.value,
                ExpireDate = favorite.expireDate,
            };
        }

        internal static object MapToModel<T>(T user)
        {
            throw new NotImplementedException();
        }
    }
}