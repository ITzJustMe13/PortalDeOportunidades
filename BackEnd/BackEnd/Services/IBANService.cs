using IbanNet;

namespace BackEnd.Services
{
    public class IBANService
    {
        /// <summary>
        /// Function to validate the IBAN of the user
        /// </summary>
        /// <param name="IBAN"></param>
        /// <returns>Returns true if the IBAN is valid, false if is not</returns>
        public static bool ValidateIBAN (string IBAN)
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
