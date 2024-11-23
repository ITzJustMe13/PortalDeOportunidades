using BackEnd.Interfaces;
using IbanNet;

namespace BackEnd.Services
{
    public class IBANService : IIBanService
    {
        public bool ValidateIBAN (string IBAN)
        {
            IIbanValidator validator = new IbanValidator();
            ValidationResult validationResult = validator.Validate(IBAN);
            if (validationResult.IsValid)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
