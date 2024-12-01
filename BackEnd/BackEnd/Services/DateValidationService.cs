using BackEnd.Interfaces;

namespace BackEnd.Services
{
    public class DateValidationService
    {
        public void ValidateAndUpdateDate(IExpirable entity)
        {
            if (entity.date < DateTime.Now)
            {
                entity.isActive = false; // Marca como desativado quando a data expira
            }
        }
    }
}
