using BackEnd.Interfaces;
using IbanNet;

namespace BackEnd.Services
{
    /// <summary>
    /// This class is responsible for the logic of the User's IBAN
    /// and implements the IIbanService Interface
    /// </summary>
    public class IBANService : IIBanService
    {
        /// <summary>
        /// Function that validates the user's IBAN through IbanValidator
        /// </summary>
        /// <param name="IBAN"></param>
        /// <returns>Returns true if the IBAN is valid, false if is not</returns>
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
