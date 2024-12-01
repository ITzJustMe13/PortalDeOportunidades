using BackEnd.Interfaces;

namespace BackEnd.Services
{
    public class DateValidationService
    {
        public void ValidateAndUpdateDate(IExpirable entity)
        {
            if (entity.Date < DateTime.Now)
            {
                entity.IsActive = false; // Marca como desativado quando a data expira
            }
        }
    }
}
